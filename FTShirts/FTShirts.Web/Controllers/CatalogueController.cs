using FTShirts.Web.enums;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using FTShirts.Web.Services;
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
        private ICatalogueService _catalogueService;
        public CatalogueController(ICatalogueService catalogueService)
        {
            _catalogueService = catalogueService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CatalogueItem>> Index()
        {
            return Ok(_catalogueService.GetAllItems());
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<CatalogueItem>> Search([FromQuery] string? team, [FromQuery] bool? onlyOffer, [FromQuery] IEnumerable<string>? tags)
        {
            bool memberLogged = true;
            if (string.IsNullOrEmpty(Request.Cookies["UserLogged"]))
            {
                memberLogged = false;
            }

            return Ok(_catalogueService.SearchItems(team, onlyOffer, tags, memberLogged));
        }

        [HttpGet("{id}")]
        public ActionResult<CatalogueItem> GetById(int id)
        {
            var item = _catalogueService.GetItemById(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] AddShirtRequest request)
        {
            _catalogueService.AddItem(request);
            return Ok();
        }
    }
}

