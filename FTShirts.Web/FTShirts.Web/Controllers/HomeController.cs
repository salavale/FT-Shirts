using FTShirts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;

namespace FTShirts.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var shirts = new List<Shirt>();
            shirts.Add(new Shirt {Id = 1, Team = "Boca Juniors", Price = 89.99m, IsSecondHand = false, Size = SSize.M });
            shirts.Add(new Shirt { Id = 2, Team = "Real Madrid", Price = 45.00m, IsSecondHand = true, PNumber = 7, PName = "Ronaldo", Size = SSize.M });
            Shirt itemShirt = new Shirt { Id = 3, Team = "Arsenal", Price = 95.50m, IsSecondHand = false, PNumber = 21, Size = SSize.M };
            CatalogueItem item = new CatalogueItem(shirt: itemShirt, offerType: Offer.Yes, discount: 20);
            Catalogue storeCatalogue = new Catalogue(shirts);
            storeCatalogue.AddItem(item);
            return View(storeCatalogue);
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
