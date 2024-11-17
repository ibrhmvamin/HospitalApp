using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Entities
{
    public class Review : BaseEntity
    {
        public int Rating { get; set; }
        public string Content { get; set; }
        public string PatientId { get; set; }
        public string DoctorId { get; set; }

        public AppUser Doctor { get; set; }
        public AppUser Patient { get; set; }
    }
}
