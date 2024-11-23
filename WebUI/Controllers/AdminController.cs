using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebUI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles ="admin")]
    public class AdminController : ControllerBase
    {
        [HttpGet("users")]
        public IActionResult GetUsers()
        {
            // Simulate fetching user data (replace with real logic)
            var users = new List<object>
    {
        new { Id = 1, Name = "John Doe", Role = "User" },
        new { Id = 2, Name = "Jane Smith", Role = "Admin" }
    };

            return Ok(users);
        }

    }
}
