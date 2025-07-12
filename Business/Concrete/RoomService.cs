using AutoMapper;
using Business.Abstract;
using Business.Dtos.RoomDto;
using Business.Exceptions;
using DataAccess.Data;
using DataAccess.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Business.Concrete;

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

    public async Task<MessageReturnDto> SendMessageAsync(NewMessageDto newMessageDto)
    {
        var sender = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == newMessageDto.SenderId);
        var receiver = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == newMessageDto.ReceiverId);

        if (sender == null || receiver == null)
            throw new CustomException(400, "Invalid sender or receiver ID.");

        bool senderIsMember = await _userManager.IsInRoleAsync(sender, "member");
        bool receiverIsMember = await _userManager.IsInRoleAsync(receiver, "member");
        bool senderIsDoctor = await _userManager.IsInRoleAsync(sender, "doctor");
        bool receiverIsDoctor = await _userManager.IsInRoleAsync(receiver, "doctor");

        if ((senderIsMember && receiverIsMember) || (senderIsDoctor && receiverIsDoctor))
            throw new CustomException(400, "Invalid users for messaging.");

        var newMessage = new Message
        {
            Id = Guid.NewGuid(),
            SenderId = newMessageDto.SenderId,
            ReceiverId = newMessageDto.ReceiverId,
            Content = newMessageDto.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _context.Messages.AddAsync(newMessage);
        await _context.SaveChangesAsync();

        return _mapper.Map<MessageReturnDto>(newMessage);
    }

    public async Task<IEnumerable<MessageReturnDto>> GetMessagesAsync(string userId)
    {
        AppUser? receiver = await _userManager.Users.SingleOrDefaultAsync(u => u.Id == userId);
        if (receiver == null) throw new CustomException(404, "User not found");
        IEnumerable<Message> messages = await _context.Messages.Where(m => m.SenderId == userId || m.ReceiverId==userId).AsNoTracking().ToListAsync();
        return _mapper.Map<IEnumerable<MessageReturnDto>>(messages);
    }
    public async Task DeleteMessageAsync(Guid messageId, string currentUserId)
    {
        var msg = await _context.Messages.FindAsync(messageId)
                  ?? throw new CustomException(404, "Message not found");

        bool isOwner = msg.SenderId == currentUserId || msg.ReceiverId == currentUserId;
        bool isAdmin = (await _userManager.GetRolesAsync(
                           await _userManager.FindByIdAsync(currentUserId)))
                           .Contains("admin");

        if (!isOwner && !isAdmin)
            throw new CustomException(403, "Not allowed");

        msg.IsDeleted = true;             
        _context.Messages.Update(msg);  

        await _context.SaveChangesAsync();
    }
    public async Task DeleteConversationAsync(string userA, string userB, string currentUserId)
    {
        if (currentUserId != userA && currentUserId != userB)
        {
            var roles = await _userManager.GetRolesAsync(
                            await _userManager.FindByIdAsync(currentUserId));
            if (!roles.Contains("admin"))
                throw new CustomException(403, "Not allowed");
        }

        var messages = await _context.Messages
            .Where(m => (m.SenderId == userA && m.ReceiverId == userB) ||
                        (m.SenderId == userB && m.ReceiverId == userA))
            .ToListAsync();

        foreach (var m in messages)
            m.IsDeleted = true; 

        await _context.SaveChangesAsync();
    }

}