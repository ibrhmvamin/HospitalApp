using Business.Abstract;
using Business.Dtos.AuthenticationDto;
using Business.Exceptions;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;

        public AuthenticationService(UserManager<AppUser> userManager, ITokenService tokenService, IEmailService emailService)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task RegisterAsync(SignUpDto signUpDto)
        {
            if (signUpDto.Password != signUpDto.PasswordConfirm) throw new CustomException(400, "Passwords do not match");
            AppUser user = new() { Name = signUpDto.Name, Email = signUpDto.Email, Surname = signUpDto.Surname, UserName = signUpDto.Email };
            IdentityResult result = await _userManager.CreateAsync(user, signUpDto.Password);
            if (!result.Succeeded) throw new CustomException(400, result.Errors.First().Code.ToString());
            if (signUpDto.Profile != null)
            {
                if (!signUpDto.Profile.IsImage()) throw new CustomException(400, "Invalid file format");
                if (signUpDto.Profile.DoesSizeExceed(100 * 1024)) throw new CustomException(400, "File size exceeds the limit");
                string filename = await signUpDto.Profile.SaveFileAsync();
                user.Profile = filename;
            }
            await _userManager.AddToRoleAsync(user, "member");
            string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            string link = $"http://localhost:5174/verify-email?token={token}&email={user.Email}";

            string body = "";
            using (StreamReader stream = new("wwwroot/templates/verifyEmail.html"))
            {
                body = stream.ReadToEnd();
            };
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{username}}", $"{user.Name} {user.Surname}");

            _emailService.SendEmail(user.Email, "Verify Email", body);
        }

        public async Task<string> LoginAsync(LoginDto loginDto)
        {
            AppUser? user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == loginDto.Email);
            if (user == null) throw new CustomException(400, "User not found");

            bool doesPasswordMatch = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!doesPasswordMatch) throw new CustomException(400, "Incorrect credentials");

            if (!user.EmailConfirmed) throw new CustomException(400, "Confirm your email");

            var nowUtc = DateTime.UtcNow;
            if (user.IsBanned || (user.BannedUntil.HasValue && user.BannedUntil > nowUtc))
            {
                string msg = user.BannedUntil.HasValue
                    ? $"You are banned until {user.BannedUntil.Value:yyyy-MM-dd HH:mm:ss} (UTC)."
                    : "You are permanently banned.";
                throw new CustomException(403, msg);
            }

            string token = _tokenService.GenerateJWTToken(user);
            return token;
        }


        public async Task ConfirmEmailAsync(ConfirmEmailDto confirmEmailDto)
        {
            AppUser? appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == confirmEmailDto.Email);
            if (appUser == null) throw new CustomException(400, $"Unable to load user {confirmEmailDto.Email}");
            await _userManager.ConfirmEmailAsync(appUser, confirmEmailDto.Token);
            appUser.EmailConfirmed = true;
            await _userManager.UpdateSecurityStampAsync(appUser);
        }

        public async Task ForgotPasswordAsync(string email)
        {
            AppUser? appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (appUser == null) throw new CustomException(404, "User does not exist");
            string token = await _userManager.GeneratePasswordResetTokenAsync(appUser);


            string body = "";
            NameValueCollection queryString = System.Web.HttpUtility.ParseQueryString(string.Empty);

            queryString.Add("token", token);
            queryString.Add("email", email);

            string link = $"http://localhost:5174/reset-password?{queryString}";

            using (StreamReader stream = new("wwwroot/templates/verifyEmail.html"))
            {
                body = stream.ReadToEnd();
            };
            body = body.Replace("{{link}}", link);
            body = body.Replace("{{username}}", $"{appUser.Name} {appUser.Surname}");

            _emailService.SendEmail(appUser.Email, "Reset Password", body);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
        {
            AppUser? appUser = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == resetPasswordDto.Email);
            if (appUser == null) throw new CustomException(404, "User does not exist");
            var result = await _userManager.ResetPasswordAsync(appUser, resetPasswordDto.Token, resetPasswordDto.Password);
            if (!result.Succeeded) throw new CustomException(400, "Expired Token");
            await _userManager.UpdateSecurityStampAsync(appUser);
        }
    }
}