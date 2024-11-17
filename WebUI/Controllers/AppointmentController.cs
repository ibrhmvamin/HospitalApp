using Business.Abstract;
using Business.Dtos.AppointmentDto;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebUI.Hubs;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly DataContext _context;

        public AppointmentController(IAppointmentService appointmentService, UserManager<AppUser> userManager, IHubContext<ChatHub> hubContext, DataContext context)
        {
            _appointmentService = appointmentService;
            _userManager = userManager;
            _hubContext = hubContext;
            _context = context;
        }

        [HttpPost("")]
        [Authorize(Roles = "member")]
        public async Task<IActionResult> Create(AppointmentCreateDto createAppointmentDto)
        {
            AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == User.Identity.Name);
            if (appUser == null || !await _userManager.IsInRoleAsync(appUser, "member")) return Unauthorized();
            await _appointmentService.CreateAsync(createAppointmentDto, appUser.Id);
            return Ok();
        }

        [HttpPut("{appointmentId}")]
        [Authorize(Roles = "doctor")]
        public async Task<IActionResult> UpdateStatus(string appointmentId, string status)
        {
            AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            AppUser? patient = _context.Appointments.Include(a => a.Patient).SingleOrDefaultAsync(a => a.Id.ToString() == appointmentId).Result.Patient;
            if (appUser == null) return NotFound();
            if (!Enum.TryParse(status, out Status result)) return BadRequest();
            await _appointmentService.ChangeStatusAsync(appointmentId, result);

            if (patient?.ClientId != null && patient != null)
            {
                var message = $"Your appointment status has been updated to {status}";
                await _hubContext.Clients.Client(patient.ClientId).SendAsync("ReceiveStatusUpdate", appointmentId, status);
            }

            return Ok();
        }

        [HttpGet("")]
        [Authorize(Roles = "member, doctor")]
        public async Task<IActionResult> Appointments()
        {
            AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (appUser == null) return Unauthorized();
            IEnumerable<AppointmentReturnDto> appointments = await _appointmentService.GetAppointmentsAsync(appUser.Id);
            return Ok(appointments);
        }
    }
}