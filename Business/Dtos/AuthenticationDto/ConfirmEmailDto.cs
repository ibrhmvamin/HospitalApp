using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Dtos.AuthenticationDto
{
    public class ConfirmEmailDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
    }
}
