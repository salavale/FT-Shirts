using FTShirts.Web.enums;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Services;

namespace FTShirts.Tests.Unit
{
    public class CartServiceTests
    {
        private static readonly CatalogueItem Boca =
            new CatalogueItem(new Shirt(2, "Boca Juniors", 45m, true, 10, "Maradona", Size.M), Offer.Yes, 20);

        // Each test gets its own CartService with its own store of logged-in users,
        // so tests never share carts
        private readonly CartService _cartService = new CartService(new FakeCatalogueService(Boca), new ActiveUserStore());
        private readonly string _username = $"user-{Guid.NewGuid():N}";

        [Fact]
        public void AddItemToCart_ExistingShirt_AddsItToTheUsersCart()
        {
            _cartService.AddItemToCart(_username, 2);

            var user = _cartService.GetCartForUser(_username);
            Assert.Equal(_username, user.Username);
            Assert.Equal(2, Assert.Single(user.ShoppingCart.Items).TShirt.Id);
        }

        [Fact]
        public void AddItemToCart_UnknownShirt_LeavesTheCartEmpty()
        {
            _cartService.AddItemToCart(_username, 99);

            Assert.Empty(_cartService.GetCartForUser(_username).ShoppingCart.Items);
        }

        [Fact]
        public void AddItemToCart_NoUsername_DoesNothing()
        {
            _cartService.AddItemToCart("", 2);

            Assert.Equal("dummy", _cartService.GetCartForUser("").Username);
        }

        [Fact]
        public void GetCartForUser_UserNotActive_ReturnsEmptyDummyUser()
        {
            var user = _cartService.GetCartForUser(_username);

            Assert.Equal("dummy", user.Username);
            Assert.Empty(user.ShoppingCart.Items);
        }

        [Fact]
        public void AddItemToCart_ManyRequestsAtTheSameTime_OneCartWithEveryShirt()
        {
            Parallel.For(0, 100, _ => _cartService.AddItemToCart(_username, 2));

            Assert.Equal(100, _cartService.GetCartForUser(_username).ShoppingCart.Items.Count);
        }
    }
}