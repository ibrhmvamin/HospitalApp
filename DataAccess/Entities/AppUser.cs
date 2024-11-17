using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class AppUser : IdentityUser
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime BirthDate { get; set; }
        public string? Description { get; set; }
        public string? Profile { get; set; }
        public List<Message> SentMessages { get; set; }
        public List<Message> ReceivedMessages { get; set; }
        public List<Appointment> AppointmentAsPatient { get; set; }
        public List<Appointment> AppointmentAsDoctor { get; set; }
        public string? ClientId { get; set; }
    }
}
