using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.AuthenticationDto
{
    public class SignUpDto
    {
        [Required, MaxLength(20)]
        public string Name { get; set; }

        [Required, MaxLength(20)]
        public string Surname { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        [Required, MinLength(6), MaxLength(25)]
        public string Password { get; set; }

        [Compare("Password")]
        public string PasswordConfirm { get; set; }

        [MaxLength(200)]
        public string? Description { get; set; }

        public IFormFile? Profile { get; set; }

        public decimal? Price { get; set; }
    }
}
