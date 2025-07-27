using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.UserDtos
{
    public class UserUpdateDto
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; }

        [Required]
        [MaxLength(20)]
        public string Surname { get; set; }


        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public IFormFile? Profile { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [StringLength(25, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 25 characters.")]
        public string? NewPassword { get; set; }

        public string? ConfirmPassword { get; set; }
    }
}
