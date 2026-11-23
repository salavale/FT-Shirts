using FTShirts.Web.Data;
using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.UserModels;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class CartService : ICartService
    {
        private readonly ICatalogueService _catalogueService;
        private readonly FTShirtsContext _context;

        public CartService(ICatalogueService catalogueService, FTShirtsContext context)
        {
            _catalogueService = catalogueService;
            _context = context;
        }

        public User GetCartForUser(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new User();
            }

            var user = _context.Users
                .Include(u => u.ShoppingCart)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(i => i.TShirt)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return new User();
            }

            user.ShoppingCart ??= new ShoppingCart { Username = username };
            return user;
        }

        public void AddItemToCart(string username, int shirtId)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            var user = _context.Users
                .Include(u => u.ShoppingCart)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(i => i.TShirt)
                .FirstOrDefault(u => u.Username == username);

            if (user == null)
            {
                return;
            }

            if (user.ShoppingCart == null)
            {
                user.ShoppingCart = new ShoppingCart { Username = username };
            }

            var itemToAdd = _catalogueService.GetItemById(shirtId);
            if (itemToAdd != null)
            {
                user.ShoppingCart.AddItem(itemToAdd);
                _context.SaveChanges();
            }
        }
    }
}