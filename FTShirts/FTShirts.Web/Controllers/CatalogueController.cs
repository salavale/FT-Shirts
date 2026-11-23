using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using FTShirts.Web.Exceptions;
using FTShirts.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FTShirts.Web.Controllers
{
    [ApiController]
    [Route("api/catalogue")]
    [Produces("application/json")]
    public class CatalogueController : ControllerBase
    {
        private readonly ICatalogueService _catalogueService;

        public CatalogueController(ICatalogueService catalogueService)
        {
            _catalogueService = catalogueService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CatalogueItem>> Index()
        {
            return Ok(_catalogueService.GetAllItemsAsync());
        }

        [HttpGet("search")]
        public ActionResult<IEnumerable<CatalogueItem>> Search([FromQuery] string? team, [FromQuery] bool? onlyOffer, [FromQuery] IEnumerable<string>? tags)
        {
            bool memberLogged = !string.IsNullOrEmpty(Request.Cookies["UserLogged"]);

            try
            {
                var results = _catalogueService.SearchItemsAsync(team, onlyOffer, tags, memberLogged);
                return Ok(results);
            }
            catch (InvalidTeamSearchException ex)
            {
                return BadRequest(new
                {
                    error = "InvalidTeamSearch",
                    team = ex.Team,
                    message = ex.Message
                });
            }
        }

        [HttpGet("{id}")]
        public ActionResult<CatalogueItem> GetById(int id)
        {
            var item = _catalogueService.GetItemByIdAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return Ok(item);
        }

        [HttpPost]
        public IActionResult AddItem([FromBody] AddShirtRequest request)
        {
            _catalogueService.AddItemAsync(request);
            return Ok();
        }
    }
}