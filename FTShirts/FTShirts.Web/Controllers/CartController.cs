using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.CatalogueModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using FTShirts.Web.Services;

namespace FTShirts.Web.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        private ICartService _cartService;
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public ActionResult<User> GetCart()
        {
            string username = Request.Cookies["UserLogged"];
            return Ok(_cartService.GetCartForUser(username));
        }


        [HttpPost("add/{shirtId}")]
        public IActionResult AddToCart(int shirtId)
        {
            string username = Request.Cookies["UserLogged"];

            _cartService.AddItemToCart(username, shirtId);
            return Ok();
        }
    }
}