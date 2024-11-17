using Business.Abstract;
using Business.Dtos.UserDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("doctors")]
        public async Task<IActionResult> GetDoctors()
        {
            IEnumerable<UserReturnDto> doctors = await _userService.GetAllDoctorsAsync();
            return Ok(doctors);
        }

        [HttpGet("patients")]
        public async Task<IActionResult> GetPatients()
        {
            IEnumerable<UserReturnDto> doctors = await _userService.GetAllPatientAsync();
            return Ok(doctors);
        }

        [HttpGet("doctor/{id}")]
        public async Task<IActionResult> GetDoctor(string id)
        {
            DoctorReturnDto dto = await _userService.GetDoctorAsync(id);
            return Ok();
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
    }
}
