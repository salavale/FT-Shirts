using FTShirts.Web.enums;
using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.CatalogueModels;

namespace FTShirts.Tests.Unit
{
    public class ShoppingCartTests
    {
        private static readonly CatalogueItem Discounted = Item(1, 100m, Offer.Yes, 20);      // 80 for everyone
        private static readonly CatalogueItem MemberDeal = Item(2, 50m, Offer.MemberOnly, 10); // 50, members pay 45
        private static readonly CatalogueItem Regular = Item(3, 30m, Offer.No, null);          // 30 for everyone

        [Fact]
        public void GetTotalPrice_Guest_OnlyGetsGeneralOffers()
        {
            var cart = CartWith(Discounted, MemberDeal, Regular);

            Assert.Equal(160m, cart.GetTotalPrice(member: false));
        }

        [Fact]
        public void GetTotalPrice_Member_AlsoGetsMemberOffers()
        {
            var cart = CartWith(Discounted, MemberDeal, Regular);

            Assert.Equal(155m, cart.GetTotalPrice(member: true));
        }

        [Fact]
        public void GetTotalPrice_EmptyCart_IsZero()
        {
            Assert.Equal(0m, new ShoppingCart().GetTotalPrice(member: false));
        }

        [Fact]
        public void RemoveItem_ShirtInCart_RemovesIt()
        {
            var cart = CartWith(Discounted, Regular);

            cart.RemoveItem(1);

            Assert.Equal(3, Assert.Single(cart.Items).TShirt.Id);
        }

        [Fact]
        public void RemoveItem_ShirtNotInCart_KeepsCartUnchanged()
        {
            var cart = CartWith(Discounted, Regular);

            cart.RemoveItem(99);

            Assert.Equal(2, cart.Items.Count);
        }

        [Fact]
        public void Clear_EmptiesTheCart()
        {
            var cart = CartWith(Discounted, Regular);

            cart.Clear();

            Assert.Empty(cart.Items);
        }

        private static ShoppingCart CartWith(params CatalogueItem[] items)
        {
            var cart = new ShoppingCart();
            foreach (var item in items)
            {
                cart.AddItem(item);
            }
            return cart;
        }

        private static CatalogueItem Item(int id, decimal price, Offer offer, int? discount)
        {
            return new CatalogueItem(new Shirt(id, "Test FC", price, false, null, null, Size.M), offer, discount);
        }
    }
}