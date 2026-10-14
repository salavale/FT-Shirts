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
        private static List<User> _activeUsers = new List<User>();

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
                if (!_activeUsers.Any(u => u.Username == request.Username))
                {
                    var newUser = new User(request.Username);
                    _activeUsers.Add(newUser);
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
                    return Conflict(new {message = $"Existing user with '{newRequest.Username}' username taken." });
                }

            }
            System.IO.File.AppendAllText(FilePath, newRequest.Username + "," + newRequest.Password + "\n");
            return Ok();
        }

        [HttpPost("addToCart/{shirtId}")]
        public IActionResult AddToCart(int shirtId)
        { 
            string username = Request.Cookies["UserLogged"];

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            if (!_activeUsers.Any(u => u.Username == username))
            {
                _activeUsers.Add(new User(username));
            }

            User activeUser = _activeUsers.First(m => m.Username == username);
            CatalogueItem? itemToAdd = CatalogueController.FindItemById(shirtId);

            if (itemToAdd != null)
            {
                activeUser.ShoppingCart.Add(itemToAdd.Value);
            }

            return Ok();
        }

        [HttpGet("shoppingCart")]
        public ActionResult<User> ShoppingCart()
        {
            string user = Request.Cookies["UserLogged"];

            if (string.IsNullOrEmpty(user) || !_activeUsers.Any(u => u.Username == user))
            {
                return Ok(new User());
            }

            User activeUser = _activeUsers.First(u => u.Username == user);

            return Ok(activeUser);
            
        }
    }
}
