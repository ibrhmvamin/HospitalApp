using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.UserDtos
{
    public class DoctorCreateDto
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

        [Required]
        [MinLength(6)]
        [MaxLength(25)]
        public string Password { get; set; }

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string PasswordConfirm { get; set; }

        public IFormFile Profile { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }

        [Required]
        public DateTime BirthDate { get; set; }

        [Range(50, 200, ErrorMessage = "Price must be between 50 and 200.")]
        public decimal Price { get; set; }
    }
}
