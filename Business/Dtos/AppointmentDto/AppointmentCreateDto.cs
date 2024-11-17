using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.AppointmentDto
{
    public class AppointmentCreateDto
    {
        [Required]
        public string StartTime { get; set; }
        [Required]
        public string DoctorId { get; set; }
    }
}
