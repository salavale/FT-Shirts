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
        public async Task<ActionResult<IEnumerable<CatalogueItem>>> Index()
        {
            return Ok(await _catalogueService.GetAllItemsAsync());
        }

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<CatalogueItem>>> Search([FromQuery] string? team, [FromQuery] bool? onlyOffer, [FromQuery] IEnumerable<string>? tags)
        {
            bool memberLogged = true;
            if (string.IsNullOrEmpty(Request.Cookies["UserLogged"]))
            {
                memberLogged = false;
            }

            return Ok(await _catalogueService.SearchItemsAsync(team, onlyOffer, tags, memberLogged));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CatalogueItem>> GetById(int id)
        {
            var item = await _catalogueService.GetItemByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> AddItem([FromBody] AddShirtRequest request)
        {
            await _catalogueService.AddItemAsync(request);
            return Ok();
        }
    }
}