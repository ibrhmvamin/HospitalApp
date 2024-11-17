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
    }
}
