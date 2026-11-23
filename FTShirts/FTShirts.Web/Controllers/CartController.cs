using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace FTShirts.Web.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        public ActionResult<User> GetCart()
        {
            string? username = Request.Cookies["UserLogged"];
            return Ok(_cartService.GetCartForUserAsync(username ?? string.Empty));
        }

        [HttpPost("add/{shirtId}")]
        public IActionResult AddToCart(int shirtId)
        {
            string? username = Request.Cookies["UserLogged"];

            _cartService.AddItemToCartAsync(username ?? string.Empty, shirtId);
            return Ok();
        }
    }
}