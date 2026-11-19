using FTShirts.Web.enums;
using FTShirts.Web.Models.CatalogueModels;

namespace FTShirts.Tests.Unit
{
    public class CatalogueItemTests
    {
        [Fact]
        public void CsvConstructor_FullLine_ReadsShirtOfferAndTags()
        {
            var item = new CatalogueItem("2,Boca Juniors,45.00,true,10,Maradona,M,Yes,20,season");

            Assert.Equal(new Shirt(2, "Boca Juniors", 45.00m, true, 10, "Maradona", Size.M), item.TShirt);
            Assert.Equal(Offer.Yes, item.OfferType);
            Assert.Equal(20, item.OfferDiscount);
            Assert.Equal(new[] { "season" }, item.Tags);
        }

        [Fact]
        public void CsvConstructor_EmptyOptionalFields_ReadsThemAsNull()
        {
            var item = new CatalogueItem("1,Real Madrid,89.99,false,,,L,No,,retro,laLiga");

            Assert.Null(item.TShirt.PNumber);
            Assert.Null(item.TShirt.PName);
            Assert.Null(item.OfferDiscount);
            Assert.Equal(new[] { "retro", "laLiga" }, item.Tags);
        }

        [Fact]
        public void ToString_ThenCsvConstructor_GivesTheSameItem()
        {
            var original = new CatalogueItem("5,Manchester United,60.00,true,7,Ronaldo,XL,Yes,30,premier,season");

            var copy = new CatalogueItem(original.ToString());

            Assert.Equal(original.TShirt, copy.TShirt);
            Assert.Equal(original.OfferType, copy.OfferType);
            Assert.Equal(original.OfferDiscount, copy.OfferDiscount);
            Assert.Equal(original.Tags, copy.Tags);
        }

        [Fact]
        public void Prices_GeneralOffer_DiscountForEveryone()
        {
            var item = CreateItem(100m, Offer.Yes, discount: 20);

            Assert.Equal(80m, item.FinalPrice);
            Assert.Equal(80m, item.MemberFinalPrice);
        }

        [Fact]
        public void Prices_MemberOnlyOffer_DiscountOnlyForMembers()
        {
            var item = CreateItem(100m, Offer.MemberOnly, discount: 25);

            Assert.Equal(100m, item.FinalPrice);
            Assert.Equal(75m, item.MemberFinalPrice);
        }

        [Fact]
        public void Prices_NoOffer_KeepOriginalPrice()
        {
            var item = CreateItem(59.90m, Offer.No);

            Assert.Equal(59.90m, item.FinalPrice);
            Assert.Equal(59.90m, item.MemberFinalPrice);
        }

        [Fact]
        public void AddTag_AddsTagToTheItem()
        {
            var item = CreateItem(10m, Offer.No);

            item.AddTag("retro");

            Assert.Contains("retro", item.Tags);
        }

        private static CatalogueItem CreateItem(decimal price, Offer offer, int? discount = null)
        {
            return new CatalogueItem(new Shirt(1, "Test FC", price, false, null, null, Size.M), offer, discount);
        }
    }
}