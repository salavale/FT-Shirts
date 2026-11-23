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
        public async Task<IActionResult> Login([FromBody] LogRegRequest request)
        {
            bool loginSuccess = await _userService.ValidateLoginAsync(request);

            if (loginSuccess)
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = false,
                    Secure = false,
                    SameSite = SameSiteMode.Lax,
                    Path = "/"
                };

                Response.Cookies.Append("UserLogged", request.Username, cookieOptions);
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
        public async Task<IActionResult> Register([FromBody] LogRegRequest newRequest)
        {
            bool success = await _userService.RegisterUserAsync(newRequest);

            if (!success)
            {
                return Conflict(new { message = $"Existing user with '{newRequest.Username}' username taken." });
            }
            return Ok();
        }
    }
}