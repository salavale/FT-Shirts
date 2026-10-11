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
        public IActionResult Index()
        {
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
        public IActionResult SSearch( string team, bool onlyOffer)
        {
            var results = _storeCatalogue.GetItemsByTeam(team: team);

            if (onlyOffer)
            {
                results = results.GetItemsOnSale();
            }

            return View("Index", results);
        }
        public IActionResult MemberPage()
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
