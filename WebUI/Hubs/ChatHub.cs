using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace WebUI.Hubs
{
    public class ChatHub : Hub
    {
        private readonly UserManager<AppUser> _userManager;

        public ChatHub(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
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
        public async Task SendMessage(string message, string recipientId)
        {
            // Get the current user's identity
            var senderEmail = Context.User?.Identity?.Name;
            if (senderEmail == null) return;

            // Find the sender in the database
            var senderUser = await _userManager.FindByNameAsync(senderEmail);
            if (senderUser == null) return;

            // Find the recipient by their ID
            var recipientUser = await _userManager.FindByIdAsync(recipientId);
            if (recipientUser == null || string.IsNullOrEmpty(recipientUser.ClientId))
            {
                await Clients.Caller.SendAsync("ReceiveMessage", "Recipient not available.", senderUser.Id);
                return;
            }

            // Send the message to the recipient's client
            await Clients.Client(recipientUser.ClientId).SendAsync("ReceiveMessage", message, senderUser.Id);
        }

    }
}
