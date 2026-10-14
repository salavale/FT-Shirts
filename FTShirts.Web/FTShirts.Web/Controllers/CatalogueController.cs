using FTShirts.Web.enums;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Xml.Linq;

namespace FTShirts.Web.Controllers
{
    public class CatalogueController : Controller
    {
        private static Catalogue _storeCatalogue = new Catalogue();
        private const string FilePath = "./Data/shirts.csv";
        //We load the whole catalogue at once.
        public CatalogueController()
        {
            if (System.IO.File.Exists(FilePath) && !_storeCatalogue.Any())
            {
                foreach (var line in System.IO.File.ReadLines(FilePath))
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

        [HttpPost]
        public IActionResult AddItem( Shirt shirt, Offer offerType, int? discount = null, IEnumerable<string>? tags = null) 
        {
            var newItem = new CatalogueItem(shirt, offerType, discount, tags);
            System.IO.File.AppendAllText(FilePath, newItem.ToString()+"\n");
            _storeCatalogue.AddItem(newItem);

            return RedirectToAction("Index");
        }
    }
}
