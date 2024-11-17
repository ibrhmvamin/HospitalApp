using Business.Abstract;
using Business.Dtos.AuthenticationDto;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] SignUpDto signUpDto)
        {
            await _authenticationService.RegisterAsync(signUpDto);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto)
        {
            string token = await _authenticationService.LoginAsync(loginDto);
            return Ok(token);
        }

        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail(ConfirmEmailDto confirmEmailDto)
        {
            await _authenticationService.ConfirmEmailAsync(confirmEmailDto);
            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            await _authenticationService.ResetPasswordAsync(resetPasswordDto);
            return Ok();
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            string token = await _authenticationService.ForgotPasswordAsync(email);
            return Ok(token);
        }
    }
}
