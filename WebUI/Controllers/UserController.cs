using Business.Abstract;
using Business.Concrete;
using Business.Dtos.AuthenticationDto;
using Business.Dtos.UserDtos;
using Business.Exceptions;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;

        public UserController(IUserService userService,UserManager<AppUser> userManager)
        {
            _userManager = userManager;
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized(); 

            var user = await _userService.GetUserProfileAsync(userId);

            if (user == null)
                return NotFound("User profile not found");

            return Ok(user);
        }

        [HttpPut("update-profile")]
        [Authorize]
        public async Task<IActionResult> UpdateProfileAsync([FromForm] UserUpdateDto userDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new CustomException(404, "User not found");

            user.Name = userDto.Name;
            user.Surname = userDto.Surname;
            user.Email = userDto.Email;
            user.BirthDate = userDto.BirthDate;

            if (userDto.Profile != null)
            {
                if (!userDto.Profile.IsImage())
                    throw new CustomException(400, "Invalid file format. Only image files are allowed.");

                if (userDto.Profile.DoesSizeExceed(100 * 1024))
                    throw new CustomException(400, "File size exceeds the limit of 100KB.");

                string filename = await userDto.Profile.SaveFileAsync();

                if (!string.IsNullOrEmpty(user.Profile))
                {
                    var oldFilePath = Path.Combine("wwwroot/", user.Profile);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                user.Profile = filename; 
            }

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new CustomException(400, result.Errors.First().Description);

            return Ok("Profile updated successfully.");
        }


        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            IEnumerable<GetUserDto> doctors = await _userService.GetAllPatientAsync();
            return Ok(doctors);
        }

        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetDoctor(string id)
        {
            DoctorReturnDto dto = await _userService.GetDoctorAsync(id);
            return Ok();
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            IEnumerable<DoctorReturnDto> doctors = await _userService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [HttpPost("doctor")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateDoctor(DoctorCreateDto doctorCreateDto)
        {
            await _userService.CreateDoctorAsync(doctorCreateDto);
            return Ok();
        }

        [HttpGet("doctor/{id}/schedule")]
        public async Task<IActionResult> GetDoctorSchedule(string id)
        {
            IEnumerable<DoctorSchdelueReturnDto> doctorReturnSchedules = await _userService.GetDoctorSchedule(id);
            return Ok(doctorReturnSchedules);
        }

        [HttpDelete("doctors/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            await _userService.DeleteDoctorAsync(id);
            return NoContent();
        }

        [HttpDelete("patients/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeletePatient(string id)
        {
            await _userService.DeletePatientAsync(id);
            return NoContent();
        }

        [HttpPut("doctors/{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromForm] UserUpdateDto dto)
        {
            var doctor = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            doctor.Name = dto.Name;
            doctor.Surname = dto.Surname;
            doctor.Email = dto.Email;
            doctor.BirthDate = dto.BirthDate;

            if (dto.Profile != null)
            {
                if (!dto.Profile.IsImage())
                    throw new CustomException(400, "Invalid file format");
                if (dto.Profile.DoesSizeExceed(100 * 1024))
                    throw new CustomException(400, "File size exceeds the limit");

                string filename = await dto.Profile.SaveFileAsync();
                doctor.Profile = filename; 
            }

            if (!string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                if (dto.NewPassword != dto.ConfirmPassword)
                    return BadRequest(new { message = "Passwords do not match" });

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(doctor);
                var pwResult = await _userManager.ResetPasswordAsync(doctor, resetToken, dto.NewPassword);

                if (!pwResult.Succeeded)
                    return BadRequest(new { message = "Password update failed", errors = pwResult.Errors });
            }

            var result = await _userManager.UpdateAsync(doctor);

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to update doctor", errors = result.Errors });

            return Ok(new { message = "Doctor updated successfully" });
        }

        [HttpPut("patients/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateUser(string id, [FromForm] UserUpdateDto userDto)
        {
            if (userDto == null)
                return BadRequest(new { message = "Invalid user data" });

            try
            { 
                await _userService.UpdateUserAsync(id, userDto);
                return Ok(new { message = "User updated successfully" });
            }
            catch (CustomException ex)
            {
                return StatusCode(ex.Code, new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An unexpected error occurred", error = ex.Message });
            }
        }

        [Authorize(Roles = "admin")]
        [HttpPut("ban/{userId}")]
        public async Task<IActionResult> BanUser(string userId, [FromQuery] DateTime? until)
        {
            await _userService.BanUserAsync(userId, until);
            return Ok(new
            {
                message = until == null
                    ? "User permanently banned."
                    : $"User banned until {until:yyyy-MM-dd HH:mm:ss} (UTC)."
            });
        }   

        [Authorize(Roles = "admin")]
        [HttpPut("unban/{userId}")]
        public async Task<IActionResult> UnbanUser(string userId)
        {
            await _userService.UnbanUserAsync(userId);
            return Ok(new { message = "User unbanned" });
        }
    }
}
