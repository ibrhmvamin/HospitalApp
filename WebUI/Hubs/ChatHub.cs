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
        public async Task SendMessage(string senderId, string receiverId, string content)
        {
            // Check if the room already exists
            var room = await _context.Rooms.FirstOrDefaultAsync(r =>
                (r.SenderId == senderId && r.ReceiverId == receiverId) ||
                (r.SenderId == receiverId && r.ReceiverId == senderId));

            // Log room existence
            Console.WriteLine($"Room found: {room != null}");

            // If the room doesn't exist, create it
            if (room == null)
            {
                room = new Room
                {
                    Id = Guid.NewGuid(),
                    SenderId = senderId,
                    ReceiverId = receiverId,
                };

                _context.Rooms.Add(room);
                await _context.SaveChangesAsync();
                Console.WriteLine($"Room created: {room.Id}");
            }

            // Ensure RoomId is correct before creating a message
            Console.WriteLine($"Using RoomId: {room.Id}");

            // Create and save the message
            var message = new Message
            {
                Content = content,
                SenderId = senderId,
                ReceiverId = receiverId,
                RoomId = room.Id,
                CreatedAt = DateTime.UtcNow
            };

            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            Console.WriteLine("Message saved");

            // Send the message to clients in the room
            await Clients.Group(room.Id.ToString()).SendAsync("ReceiveMessage", message);
        }
    }
}