using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Room : BaseEntity
    {
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
        public AppUser Sender { get; set; }
        public AppUser Receiver { get; set; }
        public ICollection<Message> Messages { get; set; } = new List<Message>();
    }
}
