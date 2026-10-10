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
        public IActionResult Index()
        {
            var items = new List<CatalogueItem>();
            string filePath = "./Data/shirts.csv";

            if (System.IO.File.Exists(filePath))
            { 
                foreach (var line in System.IO.File.ReadLines(filePath)) 
                {
                    items.Add(new CatalogueItem(line));
                }
            }
            Catalogue storeCatalogue = new Catalogue(items);

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
