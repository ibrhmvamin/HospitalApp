using Business.Abstract;
using Business.Dtos.RoomDto;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WebUI.Hubs;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IHubContext<ChatHub> _hubContext;

        public RoomController(IRoomService roomService, UserManager<AppUser> userManager, IHubContext<ChatHub> hubContext)
        {
            _roomService = roomService;
            _userManager = userManager;
            _hubContext = hubContext;
        }

        /// <summary>
        /// Send a new message to a recipient.
        /// </summary>
        /// <param name="newMessageDto">DTO containing message details</param>
        /// <returns>Status indicating success or failure</returns>
        [HttpPost("")]
        public async Task<IActionResult> SendMessage([FromBody] NewMessageDto newMessageDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                // Send the message through the service
                await _roomService.SendMessageAsync(newMessageDto);

                // Find the recipient user
                var recipient = await _userManager.Users
                    .SingleOrDefaultAsync(u => u.Id == newMessageDto.ReceiverId);
                if (recipient == null) return NotFound("Recipient not found");

                // Notify the recipient via SignalR
                if (!string.IsNullOrEmpty(recipient.ClientId))
                {
                    await _hubContext.Clients.Client(recipient.ClientId)
                        .SendAsync("NewMessage", newMessageDto.Content, newMessageDto.SenderId);
                }

                return Ok("Message sent successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        /// <summary>
        /// Retrieve messages for the logged-in user.
        /// </summary>
        /// <returns>List of messages</returns>
        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetMessages()
        {
            try
            {
                // Find the logged-in user
                var currentUser = await _userManager.Users
                    .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (currentUser == null) return NotFound("User not found");

                // Get messages for the user
                var messages = await _roomService.GetMessagesAsync(currentUser.Id);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }
    }
}
