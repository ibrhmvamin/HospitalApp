using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Message : BaseEntity
    {
        public string Content { get; set; }
        public string SenderId { get; set; }
        public string ReceiverId { get; set; }
    }

}
