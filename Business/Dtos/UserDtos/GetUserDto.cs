using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.UserDtos
{
    public class GetUserDto
    {
        public string Id { get; set; }

        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public string Profile { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }
        public bool IsBanned { get; set; }
        public DateTime? BannedUntil { get; set; }
    }
}
