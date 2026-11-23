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

        // The cart is read from and saved to the database, so these methods are asynchronous
        public async Task<User> GetCartForUserAsync(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return new User();
            }

            var user = await _context.Users
                .Include(u => u.ShoppingCart)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(i => i.TShirt)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return new User();
            }

            user.ShoppingCart ??= new ShoppingCart { Username = username };
            return user;
        }

        public async Task AddItemToCartAsync(string username, int shirtId)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            var user = await _context.Users
                .Include(u => u.ShoppingCart)
                    .ThenInclude(c => c.Items)
                        .ThenInclude(i => i.TShirt)
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return;
            }

            if (user.ShoppingCart == null)
            {
                user.ShoppingCart = new ShoppingCart { Username = username };
            }

            var itemToAdd = await _catalogueService.GetItemByIdAsync(shirtId);
            if (itemToAdd != null)
            {
                user.ShoppingCart.AddItem(itemToAdd);
                await _context.SaveChangesAsync();
            }
        }
    }
}