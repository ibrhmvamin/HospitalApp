using AutoMapper;
using Business.Abstract;
using Business.Dtos.RoomDto;
using Business.Exceptions;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class RoomService : IRoomService
    {
        private readonly DataContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public RoomService(UserManager<AppUser> userManager, DataContext context, IMapper mapper)
        {
            _userManager = userManager;
            _context = context;
            _mapper = mapper;
        }

        public async Task SendMessageAsync(NewMessageDto newMessageDto)
        {
            AppUser? sender = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == newMessageDto.SenderId);
            AppUser? receiver = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == newMessageDto.ReceiverId);
            if (sender == null || receiver == null) throw new CustomException(400, "Invalid sender or reciver id");
            Room? room = await _context.Rooms.SingleOrDefaultAsync(r => r.SenderId == newMessageDto.SenderId && r.ReceiverId == newMessageDto.ReceiverId || r.ReceiverId == newMessageDto.SenderId && r.SenderId == newMessageDto.ReceiverId);
            room ??= new() { ReceiverId = newMessageDto.ReceiverId, SenderId = newMessageDto.SenderId };
            Message newMessage = new() { RoomId = room.Id, ReceiverId = newMessageDto.ReceiverId, SenderId = newMessageDto.SenderId, Content = newMessageDto.Content };
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MessageReturnDto>> GetMessagesAsync(string userId)
        {
            AppUser? receiver = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
            if (receiver == null) throw new CustomException(404, "User not found");
            IEnumerable<Message> messages = await _context.Messages.Where(m => m.ReceiverId == userId).AsNoTracking().ToListAsync();
            return _mapper.Map<IEnumerable<MessageReturnDto>>(messages);
        }
    }
}
