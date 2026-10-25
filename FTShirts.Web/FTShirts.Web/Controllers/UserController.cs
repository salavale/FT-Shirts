using FTShirts.Web.Controllers;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace FTShirts.Web.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Produces("application/json")]
    public class UserController : ControllerBase
    {
        private const string FilePath = "./Data/users.csv";
        internal static List<User> ActiveUsers = new List<User>();

        [HttpPost("login")]
        public IActionResult Login([FromBody] LogRegRequest request)
        {
            bool loginSuccess = false;

            if (System.IO.File.Exists(FilePath))
            {
                string expectedLine = request.Username + "," + request.Password;
                loginSuccess = System.IO.File.ReadLines(FilePath).Any(line => line == expectedLine);
            }

            if (loginSuccess)
            {
                Response.Cookies.Append("UserLogged", request.Username);
                if (!ActiveUsers.Any(u => u.Username == request.Username))
                {
                    var newUser = new User(request.Username);
                    ActiveUsers.Add(newUser);
                }

                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserLogged");

            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LogRegRequest newRequest)
        {
            if (System.IO.File.Exists(FilePath))
            {
                if (System.IO.File.ReadLines(FilePath).Any(line => line.StartsWith(newRequest.Username + ",")))
                {
                    return Conflict(new { message = $"Existing user with '{newRequest.Username}' username taken." });
                }

            }
            System.IO.File.AppendAllText(FilePath, newRequest.Username + "," + newRequest.Password + "\n");
            return Ok();
        }
    }
}