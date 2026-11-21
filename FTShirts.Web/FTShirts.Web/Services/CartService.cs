using FTShirts.Web.Models.UserModels;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class CartService : ICartService
    {
        private readonly ICatalogueService _catalogueService;
        public CartService(ICatalogueService catalogueService)
        {
            _catalogueService = catalogueService;
        }

        public User GetCartForUser(string username)
        {
            if (!string.IsNullOrEmpty(username) && UserService.ActiveUsers.TryGetValue(username, out var activeUser))
            {
                return activeUser;
            }

            return new User();
        }

        public void AddItemToCart(string username, int shirtId)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            // GetOrAdd finds the user or adds it in one atomic step, so two requests
            // at the same time can never create two carts for the same user
            User activeUser = UserService.ActiveUsers.GetOrAdd(username, name => new User(name));
            var itemToAdd = _catalogueService.GetItemById(shirtId);

            if (itemToAdd != null)
            {
                activeUser.ShoppingCart.AddItem(itemToAdd.Value);
            }
        }
    }
}