// WebUI/Hubs/ChatHub.cs
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace WebUI.Hubs;

[Authorize]                             
public class ChatHub : Hub
{
    private readonly UserManager<AppUser> _userManager;
    private readonly DataContext _context;

    public ChatHub(UserManager<AppUser> userManager, DataContext context)
    {
        _userManager = userManager;
        _context = context;
    }
    public override async Task OnConnectedAsync()
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                user.ClientId = Context.ConnectionId;
                await _userManager.UpdateAsync(user);
            }
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId != null)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                user.ClientId = null;
                await _userManager.UpdateAsync(user);
            }
        }
        await base.OnDisconnectedAsync(exception);
    }
    public async Task SendMessage(string senderId, string receiverId, string messageContent)
    {
        var msg = new Message
        {
            Id = Guid.NewGuid(),
            Content = messageContent,
            CreatedAt = DateTime.UtcNow,
            SenderId = senderId,
            ReceiverId = receiverId
        };

        _context.Messages.Add(msg);
        await _context.SaveChangesAsync();
        await Clients.Users(new[] { senderId, receiverId })
                     .SendAsync("ReceiveMessage", senderId, receiverId, messageContent);
    }
}
