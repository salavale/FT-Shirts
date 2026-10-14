using FTShirts.Web.Controllers;
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
        private const string FilePath = "./Data/users.csv";
        private static List<User> _activeUsers = new List<User>();
        public IActionResult UserPage()
        {
            return View();
        }

        public IActionResult UserRegister()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            bool loginSuccess = false;

            if (System.IO.File.Exists(FilePath))
            {
                string expectedLine = username + "," + password;
                loginSuccess = System.IO.File.ReadLines(FilePath).Any(line => line == expectedLine);
            }

            if (loginSuccess)
            {
                Response.Cookies.Append("UserLogged", username);
                _activeUsers.Add(new User(username));

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
            if (System.IO.File.Exists(FilePath))
            {
                if (System.IO.File.ReadLines(FilePath).Any(line => line.StartsWith(newUsername + ",")))
                {
                    return Content("Login Failed. Username already taken.");
                }

            }
            System.IO.File.AppendAllText(FilePath, newUsername + "," + newPassword + "\n");
            return RedirectToAction("UserPage");
        }

        [HttpPost]
        public IActionResult AddToCart(int shirtId)
        { 
            string username = Request.Cookies["UserLogged"];

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToAction("UserPage");
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

        public IActionResult SellShirt()
        {
            if (string.IsNullOrEmpty(Request.Cookies["UserLogged"]))
            {
                return RedirectToAction("UserPage");
            }

            return View();
        }
    }
}
