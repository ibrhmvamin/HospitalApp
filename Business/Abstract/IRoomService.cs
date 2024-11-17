using Business.Dtos.RoomDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IRoomService
    {
        Task SendMessageAsync(NewMessageDto newMessageDto);
        Task<IEnumerable<MessageReturnDto>> GetMessagesAsync(string userId);
    }
}
