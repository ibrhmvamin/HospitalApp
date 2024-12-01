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
            AppUser? sender = await _userManager.FindByIdAsync(newMessageDto.SenderId);
            AppUser? receiver = await _userManager.FindByIdAsync(newMessageDto.ReceiverId);

            if (sender == null || receiver == null)
                return BadRequest("Invalid sender or receiver ID.");

            bool senderIsMember = await _userManager.IsInRoleAsync(sender, "member");
            bool receiverIsMember = await _userManager.IsInRoleAsync(receiver, "member");
            bool senderIsDoctor = await _userManager.IsInRoleAsync(sender, "doctor");
            bool receiverIsDoctor = await _userManager.IsInRoleAsync(receiver, "doctor");

            if ((senderIsMember && receiverIsMember) || (senderIsDoctor && receiverIsDoctor))
                return BadRequest("Invalid users for messaging.");

            var newMessage = new Message
            {
                Id = Guid.NewGuid(),
                SenderId = newMessageDto.SenderId,
                ReceiverId = newMessageDto.ReceiverId,
                Content = newMessageDto.Content,
                CreatedAt = DateTime.UtcNow
            };

            if (_context == null)
            {
                throw new NullReferenceException("DataContext is not initialized.");
            }


            await _context.Messages.AddAsync(newMessage);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.User(newMessageDto.ReceiverId)
                .SendAsync("ReceiveMessage", newMessageDto.SenderId, newMessageDto.Content);

            return Ok(new { Message = "Message sent successfully" });
        }

        [Authorize]
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages()
        {
            try
            {
                var currentUser = await _userManager.Users
                    .SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);

                if (currentUser == null) return NotFound("User not found");

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