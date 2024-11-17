using Business.Abstract;
using Business.Dtos.RoomDto;
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
        public async Task<IActionResult> SendMessage(NewMessageDto newMessageDto)
        {
            await _roomService.SendMessageAsync(newMessageDto);
            AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == newMessageDto.ReceiverId);
            if (appUser == null) return NotFound();

            if (appUser?.ClientId != null && appUser != null)
                await _hubContext.Clients.Client(appUser.ClientId).SendAsync("NewMessage", newMessageDto.Content, newMessageDto.SenderId);

            return Ok();
        }

        [Authorize]
        [HttpGet("")]
        public async Task<IActionResult> GetMessages()
        {
            AppUser? appUser = await _userManager.Users.SingleOrDefaultAsync(u => u.UserName == User.Identity.Name);
            if (appUser == null) return NotFound();
            IEnumerable<MessageReturnDto> messages = await _roomService.GetMessagesAsync(appUser.Id);
            return Ok(messages);
        }
    }
}
