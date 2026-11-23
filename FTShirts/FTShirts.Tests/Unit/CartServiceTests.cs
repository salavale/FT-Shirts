using FTShirts.Web.Data;
using FTShirts.Web.enums;
using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FTShirts.Tests.Unit
{
    public class CartServiceTests : IDisposable
    {
        private readonly FTShirtsContext _context;
        private readonly CartService _cartService;
        private readonly string _username = $"user-{Guid.NewGuid():N}";

        public CartServiceTests()
        {
            var options = new DbContextOptionsBuilder<FTShirtsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FTShirtsContext(options);
            var catalogueService = new CatalogueService(_context);
            _cartService = new CartService(catalogueService, _context);
        }

        [Fact]
        public void AddItemToCart_ExistingShirt_AddsItToTheUsersCart()
        {
            SeedUserWithCart(_username, "secret");
            SeedCatalogueItem(2, "Boca Juniors", 45m, Offer.Yes, 20);

            _cartService.AddItemToCartAsync(_username, 2);

            var user = _cartService.GetCartForUserAsync(_username);
            Assert.Equal(_username, user.Username);
            var item = Assert.Single(user.ShoppingCart.Items);
            Assert.NotNull(item.TShirt);
            Assert.Equal(2, item.TShirt.Id);
        }

        [Fact]
        public void AddItemToCart_UnknownShirt_LeavesTheCartEmpty()
        {
            SeedUserWithCart(_username, "secret");

            _cartService.AddItemToCartAsync(_username, 99);

            var user = _cartService.GetCartForUserAsync(_username);
            Assert.Empty(user.ShoppingCart.Items);
        }

        [Fact]
        public void AddItemToCart_NoUsername_DoesNothing()
        {
            SeedCatalogueItem(2, "Boca Juniors", 45m, Offer.Yes, 20);

            _cartService.AddItemToCartAsync("", 2);

            Assert.Equal("dummy", _cartService.GetCartForUserAsync("").Username);
        }

        [Fact]
        public void GetCartForUser_UserNotActive_ReturnsEmptyDummyUser()
        {
            var user = _cartService.GetCartForUserAsync(_username);

            Assert.Equal("dummy", user.Username);
            Assert.Empty(user.ShoppingCart.Items);
        }

        private void SeedUserWithCart(string username, string password)
        {
            var user = new User(username, password)
            {
                ShoppingCart = new ShoppingCart { Username = username }
            };

            _context.Users.Add(user);
            _context.SaveChanges();
        }

        private void SeedCatalogueItem(int id, string team, decimal price, Offer offer, int? discount)
        {
            var shirt = new Shirt(id, team, price, true, 10, "Maradona", Size.M);
            var item = new CatalogueItem(shirt, offer, discount);

            _context.Add(shirt);
            _context.Add(item);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}