using FTShirts.Web.Controllers;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using FTShirts.Web.Services;
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
        private IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpPost("login")]
        public IActionResult Login([FromBody] LogRegRequest request)
        {
            bool loginSuccess = _userService.ValidateLogin(request);

            if (loginSuccess)
            {
                Response.Cookies.Append("UserLogged", request.Username);
                _userService.EnsureUserActive(request.Username);
                return Ok();
            }

            return Unauthorized();
        }

        [HttpDelete("logout")]
        public IActionResult Logout()
        {
            _userService.LogOutUpdate(Request.Cookies["UserLogged"]);
            Response.Cookies.Delete("UserLogged");
            return Ok();
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] LogRegRequest newRequest)
        {
            bool success = _userService.RegisterUser(newRequest);

            if (!success)
            {
                return Conflict(new { message = $"Existing user with '{newRequest.Username}' username taken." });
            }
            return Ok();
        }
    }
}