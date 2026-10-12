using FTShirts.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.Xml.Linq;
using System.IO;
using FTShirts.Web.Models.CatalogueModels;

namespace FTShirts.Web.Controllers
{
    public class CatalogueController : Controller
    {
        private static Catalogue _storeCatalogue = new Catalogue();
        //We load the whole catalogue at once.
        public CatalogueController()
        {
            string filePath = "./Data/shirts.csv";
            if (System.IO.File.Exists(filePath) && !_storeCatalogue.Any())
            {
                foreach (var line in System.IO.File.ReadLines(filePath))
                {
                    _storeCatalogue.AddItem(new CatalogueItem(line));
                }
            }
        }
        public IActionResult Index()
        {
            return View(_storeCatalogue);
        }

        [HttpGet]
        public IActionResult Search(string team, bool onlyOffer, IEnumerable<string> tags)
        {
            string memberLogged = Request.Cookies["UserLogged"];
            
            var results = _storeCatalogue.GetItemsByTeam(team: team);

            if (onlyOffer)
            {
                if (string.IsNullOrEmpty(memberLogged))
                {
                    results = results.GetItemsOnSale();
                }
                else
                {
                    results = results.GetItemsOnSaleForMembers();
                }
            }
            results = results.GetItemsWithTags(tags);

            return View("Index", results);
        }
        public static CatalogueItem? FindItemById(int id)
        {
            return _storeCatalogue.GetItemByID(id);
        }
    }
}
