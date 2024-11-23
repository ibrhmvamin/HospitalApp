using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.UserDtos
{
    public class DoctorReturnDto
    {
        public string Id { get; set; }
        public string Profile { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public IEnumerable<string> Statuses { get; set; }
    }
}
