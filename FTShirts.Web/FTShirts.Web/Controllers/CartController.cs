using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.CatalogueModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace FTShirts.Web.Controllers
{
    [ApiController]
    [Route("api/cart")]
    [Produces("application/json")]
    public class CartController : ControllerBase
    {
        [HttpGet]
        public ActionResult<User> GetCart()
        {
            string username = Request.Cookies["UserLogged"];

            if (string.IsNullOrEmpty(username) || !UserController.ActiveUsers.Any(u => u.Username == username))
            {
                return Ok(new User());
            }

            User activeUser = UserController.ActiveUsers.First(u => u.Username == username);
            return Ok(activeUser);
        }

        [HttpPost("add/{shirtId}")]
        public IActionResult AddToCart(int shirtId)
        {
            string username = Request.Cookies["UserLogged"];

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized();
            }

            if (!UserController.ActiveUsers.Any(u => u.Username == username))
            {
                UserController.ActiveUsers.Add(new User(username));
            }

            User activeUser = UserController.ActiveUsers.First(m => m.Username == username);
            CatalogueItem? itemToAdd = CatalogueController.FindItemById(shirtId);

            if (itemToAdd != null)
            {
                activeUser.ShoppingCart.AddItem(itemToAdd.Value);
            }

            return Ok();
        }
    }
}