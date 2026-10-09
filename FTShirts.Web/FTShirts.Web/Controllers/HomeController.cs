using FTShirts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace FTShirts.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var shirts = new List<Shirt>();
            shirts.Add( new Shirt {Id = 1, Team = "Real Madrid", Price = 89.99m, IsSecondHand = false });
            shirts.Add(new Shirt { Id = 2, Team = "Boca Juniors", Price = 45.00m, IsSecondHand = true });
            shirts.Add(new Shirt { Id = 3, Team = "Arsenal", Price = 95.50m, IsSecondHand = false });

            return View(shirts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
