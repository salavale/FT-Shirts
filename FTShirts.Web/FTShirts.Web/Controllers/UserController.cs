using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace FTShirts.Web.Controllers
{
    public class UserController : Controller
    {
        private static List<User> _activeUsers = new List<User>();
        public IActionResult MemberPage()
        {
            return View();
        }

        public IActionResult MemberRegister()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string filePath = "./Data/members.csv";
            bool loginSuccess = false;

            if (System.IO.File.Exists(filePath))
            {
                string expectedLine = username + "," + password;
                loginSuccess = System.IO.File.ReadLines(filePath).Any(line => line == expectedLine);
            }

            if (loginSuccess)
            {
                Response.Cookies.Append("UserLogged", username);
                _activeUsers.Add(new Member(username));

                return RedirectToAction("Index", "Catalogue");
            }

            return Content("Login Failed. Incorrect username or password.");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserLogged");

            Response.Cookies.Delete("GuestUser");

            return RedirectToAction("Index", "Catalogue");
        }

        [HttpPost]
        public IActionResult Register(string newUsername, string newPassword)
        {
            string filePath = "./Data/members.csv";
            if (System.IO.File.Exists(filePath))
            {
                if (System.IO.File.ReadLines(filePath).Any(line => line.StartsWith(newUsername + ",")))
                {
                    return Content("Login Failed. Username already taken.");
                }

            }
            System.IO.File.AppendAllText(filePath, newUsername + "," + newPassword + "\n");
            return RedirectToAction("MemberPage");
        }

        [HttpPost]
        public IActionResult AddToCart(int shirtId)
        {
            string user = Request.Cookies["UserLogged"];
            bool activeMember = false;

            if (string.IsNullOrEmpty(user))
            {
                user = Request.Cookies["GuestUser"];
            }
            else
            {
                activeMember = true;
            }

            if (string.IsNullOrEmpty(user))
            {
                user = "Guest" + Guid.NewGuid().ToString();
                Response.Cookies.Append("GuestUser", user);
                _activeUsers.Add(new User(user));
            }
            else if (!_activeUsers.Any(u => u.Username == user))
            {
                if (activeMember)
                {
                    _activeUsers.Add(new Member(user));
                }
                else
                {
                    _activeUsers.Add(new User(user));
                }
            }

            User activeUser = _activeUsers.First(m => m.Username == user);
            CatalogueItem? itemToAdd = CatalogueController.FindItemById(shirtId);

            if (itemToAdd != null)
            {
                activeUser.ShoppingCart.Add(itemToAdd.Value);
            }

            return RedirectToAction("Index", "Catalogue");
        }

        [HttpGet]
        public IActionResult ShoppingCart()
        {
            string user = Request.Cookies["UserLogged"] ?? Request.Cookies["GuestUser"];

            if (string.IsNullOrEmpty(user) || !_activeUsers.Any(u => u.Username == user))
            {
                return View(new User());
            }

            User activeUser = _activeUsers.First(u => u.Username == user);

            return View(activeUser);
            
        }
    }
}
