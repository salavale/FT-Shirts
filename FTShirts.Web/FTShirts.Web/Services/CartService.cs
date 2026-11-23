using FTShirts.Web.Models.UserModels;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class CartService : ICartService
    {
        private readonly ICatalogueService _catalogueService;
        private readonly IActiveUserStore _activeUsers;

        // Both dependencies are injected by ASP.NET (see Program.cs), so CartService no longer
        // reaches into UserService to find the logged-in users
        public CartService(ICatalogueService catalogueService, IActiveUserStore activeUsers)
        {
            _catalogueService = catalogueService;
            _activeUsers = activeUsers;
        }

        public User GetCartForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new User();
            }

            return _activeUsers.Find(username) ?? new User();
        }

        public void AddItemToCart(string username, int shirtId)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            User activeUser = _activeUsers.GetOrAdd(username);
            var itemToAdd = _catalogueService.GetItemById(shirtId);

            if (itemToAdd != null)
            {
                activeUser.ShoppingCart.AddItem(itemToAdd.Value);
            }
        }
    }
}