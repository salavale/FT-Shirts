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
    [ApiController]
    [Route("api/catalogue")]
    [Produces("application/json")]
    public class CatalogueController : ControllerBase
    {
        private static Catalogue _storeCatalogue = new Catalogue();
        private const string FilePath = "./Data/shirts.csv";
        //We load the whole catalogue at once.
        public CatalogueController()
        {
            if (System.IO.File.Exists(FilePath) && !_storeCatalogue.Any())
            {
                foreach (var line in System.IO.File.ReadLines(FilePath).Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    _storeCatalogue.AddItem(new CatalogueItem(line));
                }
            }
        }
        [HttpGet]
        public ActionResult<IEnumerable<CatalogueItem>> Index()
        {
            return Ok(_storeCatalogue);
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<CatalogueItem>> Search([FromQuery] string? team, [FromQuery] bool? onlyOffer, [FromQuery] IEnumerable<string>? tags)
        {
            string memberLogged = Request.Cookies["UserLogged"];
            
            var results = _storeCatalogue.GetItemsByTeam(team: team);

            if (onlyOffer.HasValue)
            {
                if (onlyOffer.Value)
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
            }
            results = results.GetItemsWithTags(tags);

            return Ok(results);
        }
        internal static CatalogueItem? FindItemById(int id)
        {
            return _storeCatalogue.GetItemByID(id);
        }

        [HttpGet("{id}")]
        public ActionResult<CatalogueItem> GetById(int id)
        {
            var item = FindItemById(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] AddShirtRequest request) 
        {
            int newId = _storeCatalogue.Any() ? _storeCatalogue.Max(i => i.TShirt.Id) + 1 : 1;
            Shirt finalShirt = request.Shirt with { Id = newId, IsSecondHand = true };

            var newItem = new CatalogueItem(shirt: finalShirt, offerType: Offer.No, tags: request.Tags);

            System.IO.File.AppendAllText(FilePath, newItem.ToString()+"\n");
            _storeCatalogue.AddItem(newItem);

            return Ok();
        }
    }
}
