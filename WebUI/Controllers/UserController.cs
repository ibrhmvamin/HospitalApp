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

            // Find the user
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new CustomException(404, "User not found");

            // Update common fields
            user.Name = userDto.Name;
            user.Surname = userDto.Surname;
            user.Email = userDto.Email;
            user.BirthDate = userDto.BirthDate;

            // Handle profile image if uploaded
            if (userDto.Profile != null)
            {
                if (!userDto.Profile.IsImage())
                    throw new CustomException(400, "Invalid file format. Only image files are allowed.");

                if (userDto.Profile.DoesSizeExceed(100 * 1024))
                    throw new CustomException(400, "File size exceeds the limit of 100KB.");

                // Save the profile image and set the filename to the user's profile
                string filename = await userDto.Profile.SaveFileAsync();

                // Delete old profile picture if it exists
                if (!string.IsNullOrEmpty(user.Profile))
                {
                    var oldFilePath = Path.Combine("wwwroot/", user.Profile);
                    if (System.IO.File.Exists(oldFilePath))
                        System.IO.File.Delete(oldFilePath);
                }

                user.Profile = filename; // Update the profile image for the existing user entity
            }

            // Update the user
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
            IEnumerable<GetDoctorDto> doctors = await _userService.GetAllDoctorsAsync();
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

        [HttpPut("doctors/{id}")]
        public async Task<IActionResult> UpdateDoctor(string id, [FromForm] DoctorUpdateDto dto)
        {
            // Retrieve the doctor entity from the database
            var doctor = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == id);
            if (doctor == null)
                return NotFound(new { message = "Doctor not found" });

            // Update the doctor's properties
            doctor.Name = dto.Name;
            doctor.Surname = dto.Surname;
            doctor.Email = dto.Email;
            doctor.BirthDate = dto.BirthDate;
            doctor.Description = dto.Description;

            // Handle profile image if uploaded
            if (dto.Profile != null)
            {
                if (!dto.Profile.IsImage())
                    throw new CustomException(400, "Invalid file format");
                if (dto.Profile.DoesSizeExceed(100 * 1024))
                    throw new CustomException(400, "File size exceeds the limit");

                // Save the profile image and set the filename to the doctor's profile
                string filename = await dto.Profile.SaveFileAsync();
                doctor.Profile = filename; // Update the profile image for the existing doctor entity
            }

            // Update the doctor entity in the database
            var result = await _userManager.UpdateAsync(doctor);

            if (!result.Succeeded)
                return BadRequest(new { message = "Failed to update doctor", errors = result.Errors });

            // Return success response
            return Ok(new { message = "Doctor updated successfully" });
        }

    }
}
