using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace WebUI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly DataContext _context;

        public ChatHub(UserManager<AppUser> userManager, DataContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public override Task OnConnectedAsync()
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser? appUser = _userManager.Users.SingleOrDefaultAsync(u => u.Email == Context.User.Identity.Name).Result;
                if (appUser != null)
                {
                    appUser.ClientId = Context.ConnectionId;
                    _userManager.UpdateAsync(appUser).Wait();
                }
            }
            return base.OnConnectedAsync();
        }
        public override Task OnDisconnectedAsync(Exception? exception)
        {
            if (Context.User.Identity.IsAuthenticated)
            {
                AppUser? user = _userManager.FindByNameAsync(Context.User.Identity.Name).Result;
                if (user != null)
                {
                    user.ClientId = null;
                    _userManager.UpdateAsync(user).Wait();
                }
            }
            return base.OnDisconnectedAsync(exception);
        }
        public async Task SendMessage(string senderId, string receiverId, string messageContent)
        {
            var message = new Message
            {
                Id = Guid.NewGuid(),
                Content = messageContent,
                CreatedAt = DateTime.UtcNow,
                SenderId = senderId,
                ReceiverId = receiverId
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();

            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, messageContent);
        }

    }
}