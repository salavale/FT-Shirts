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
            if (string.IsNullOrEmpty(username) || !UserService.ActiveUsers.Any(u => u.Username == username))
            {
                return new User();
            }

            return UserService.ActiveUsers.First(u => u.Username == username);
        }

        public void AddItemToCart(string username, int shirtId)
        {
            if (!string.IsNullOrEmpty(username))
            {
                if (!UserService.ActiveUsers.Any(u => u.Username == username))
                {
                    UserService.ActiveUsers.Add(new User(username));
                }

                User activeUser = UserService.ActiveUsers.First(m => m.Username == username);
                var itemToAdd = _catalogueService.GetItemById(shirtId);

                if (itemToAdd != null)
                {
                    activeUser.ShoppingCart.AddItem(itemToAdd.Value);
                }
            }
        }
    }
}