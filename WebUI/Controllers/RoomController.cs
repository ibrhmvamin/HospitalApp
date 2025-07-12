using Business.Abstract;
using Business.Dtos.RoomDto;
using Business.Exceptions;
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
        private readonly DataContext _context;

        public RoomController(IRoomService roomService, UserManager<AppUser> userManager, IHubContext<ChatHub> hubContext, DataContext context)
        {
            _roomService = roomService;
            _userManager = userManager;
            _hubContext = hubContext;
            _context = context;
        }

        [HttpPost("")]
        public async Task<IActionResult> SendMessage([FromBody] NewMessageDto newMessageDto)
        {
            var messageDto = await _roomService.SendMessageAsync(newMessageDto);

            await _hubContext.Clients.User(newMessageDto.ReceiverId)
                .SendAsync("ReceiveMessage", newMessageDto.SenderId, newMessageDto.Content);

            return Ok(new
            {
                Message = "Message sent successfully",
                Data = messageDto
            });
        }

        [Authorize]
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (userId == null) return NotFound("User not found");

                var messages = await _roomService.GetMessagesAsync(userId);
                return Ok(messages);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"An error occurred: {ex.Message}");
            }
        }

        [HttpDelete("message/{id:guid}")]
        public async Task<IActionResult> DeleteMessage(Guid id)
        {
            var currentUserId =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _roomService.DeleteMessageAsync(id, currentUserId);
            return NoContent();
        }

        [HttpDelete("conversation/{otherUserId}")]
        public async Task<IActionResult> DeleteConversation(string otherUserId)
        {
            var currentUserId =
                User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value!;
            await _roomService.DeleteConversationAsync(currentUserId, otherUserId, currentUserId);
            return NoContent();
        }
    }
}