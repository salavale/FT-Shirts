using FTShirts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using System.IO;

namespace FTShirts.Web.Controllers
{
    public class HomeController : Controller
    {
        private static Catalogue _storeCatalogue = new Catalogue();
        private static List<User> _activeUsers = new List<User>();
        public IActionResult Index()
        {

            string memberLogged = Request.Cookies["UserLogged"];
            if (!string.IsNullOrEmpty(memberLogged))
            {
                return RedirectToAction("MemberIndex");
            }

            string filePath = "./Data/shirts.csv";
            _storeCatalogue = new Catalogue();

            if (System.IO.File.Exists(filePath))
            { 
                foreach (var line in System.IO.File.ReadLines(filePath)) 
                {
                    _storeCatalogue.AddItem(new CatalogueItem(line));
                }
            }

            return View(_storeCatalogue);
        }

        public IActionResult Offers()
        {
            var sale = _storeCatalogue.GetItemsOnSale();

            return View("Index", sale);
        }

        [HttpGet]
        public IActionResult Search( string team)
        {
            var results = _storeCatalogue.GetItemsByTeam(team);

            return View("Index", results);
        }

        [HttpGet]
        public IActionResult SSearch( string team, bool onlyOffer, IEnumerable<string> tags)
        {
            var results = _storeCatalogue.GetItemsByTeam(team: team);

            if (onlyOffer)
            {
                results = results.GetItemsOnSale();
            }

            results = results.GetItemsWithTags(tags);

            return View("Index", results);
        }
        public IActionResult MemberPage()
        {
            return View();
        }

        public IActionResult MemberRegister()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login( string username, string password)
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
                _activeUsers.Add(new Member( username ));

                return RedirectToAction("MemberIndex");
            }

            return Content("Login Failed. Incorrect username or password.");
        }

        public IActionResult Logout()
        {
            Response.Cookies.Delete("UserLogged");

            Response.Cookies.Delete("GuestUser");

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Register( string newUsername, string newPassword)
        {
            string filePath = "./Data/members.csv";
            if (System.IO.File.Exists(filePath))
            {
                if (System.IO.File.ReadLines(filePath).Any(line => line.StartsWith(newUsername + ","))){
                    return Content("Login Failed. Username already taken.");
                }

            }
            System.IO.File.AppendAllText(filePath, newUsername + "," + newPassword + "\n");
            return RedirectToAction("MemberPage");
        }

        public IActionResult MemberIndex()
        {
            string memberLogged = Request.Cookies["UserLogged"];
            if (string.IsNullOrEmpty(memberLogged))
            {
                return RedirectToAction("Index");
            }

            string filePath = "./Data/shirts.csv";
            _storeCatalogue = new Catalogue();

            if (System.IO.File.Exists(filePath))
            {
                foreach (var line in System.IO.File.ReadLines(filePath))
                {
                    _storeCatalogue.AddItem(new CatalogueItem(line));
                }
            }

            return View(_storeCatalogue);
        }

        public IActionResult MemberSSearch(string team, bool onlyOffer, IEnumerable<string> tags)
        {
            string memberLogged = Request.Cookies["UserLogged"];
            if (string.IsNullOrEmpty(memberLogged))
            {
                return RedirectToAction("SSearch", new { team, onlyOffer, tags });
            }
            var results = _storeCatalogue.GetItemsByTeam(team: team);

            if (onlyOffer)
            {
                results = results.GetItemsOnSaleForMembers();
            }
            results = results.GetItemsWithTags(tags);

            return View("MemberIndex", results);
        }

        [HttpPost]
        public IActionResult AddToCart(int shirtId)
        {
            string user = Request.Cookies["UserLogged"];
            bool activeMember = false;
            //if there is no member, we see if that user has a guest user. 
            if (string.IsNullOrEmpty(user))
            {
                user = Request.Cookies["GuestUser"];
            }
            else
            {
                activeMember = true;
            }
            //If don't have a guest user too, we create the user a guest user
            if (string.IsNullOrEmpty(user))
            {
                user = "Guest"+Guid.NewGuid().ToString();
                Response.Cookies.Append("GuestUser", user);
                _activeUsers.Add(new User(user));
            }

            User activeUser = _activeUsers.First(m => m.Username == user);

            CatalogueItem? itemToAdd = _storeCatalogue.GetItemByID(shirtId);
            if (itemToAdd != null)
            {
                activeUser.ShoppingCart.Add(itemToAdd.Value);
            }

            if (activeMember)
            {
                return View("MemberIndex");
            }
            return View("Index");
        }


            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
