using FTShirts.Web.enums;
using FTShirts.Web.Models.CatalogueModels;

namespace FTShirts.Tests.Unit
{
    public class CatalogueExtensionsTests
    {
        private static readonly CatalogueItem[] Items =
        {
            Item(1, "Real Madrid", Offer.No, null, "laLiga", "retro"),
            Item(2, "Boca Juniors", Offer.Yes, 20, "season"),
            Item(3, "Arsenal", Offer.MemberOnly, 10, "premier", "retro"),
            Item(4, "Real Betis", Offer.Yes, 15, "laLiga"),
        };

        [Fact]
        public void GetItemsOnSale_ReturnsOnlyGeneralOffers()
        {
            Assert.Equal(new[] { 2, 4 }, Ids(Items.GetItemsOnSale()));
        }

        [Fact]
        public void GetItemsOnSaleForMembers_AlsoReturnsMemberOnlyOffers()
        {
            Assert.Equal(new[] { 2, 3, 4 }, Ids(Items.GetItemsOnSaleForMembers()));
        }

        [Theory]
        [InlineData("real", new[] { 1, 4 })]
        [InlineData("REAL MADRID", new[] { 1 })]
        [InlineData("chelsea", new int[] { })]
        public void GetItemsByTeam_FindsPartOfTheNameIgnoringCase(string team, int[] expectedIds)
        {
            Assert.Equal(expectedIds, Ids(Items.GetItemsByTeam(team)));
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void GetItemsByTeam_NoTeam_ReturnsEverything(string? team)
        {
            Assert.Equal(4, Items.GetItemsByTeam(team!).Count());
        }

        [Fact]
        public void GetItemsWithTags_SeveralTags_ReturnsItemsThatHaveAllOfThem()
        {
            Assert.Equal(new[] { 1 }, Ids(Items.GetItemsWithTags(new[] { "laLiga", "retro" })));
        }

        [Fact]
        public void GetItemsWithTags_NoTags_ReturnsEverything()
        {
            Assert.Equal(4, Items.GetItemsWithTags(Array.Empty<string>()).Count());
            Assert.Equal(4, Items.GetItemsWithTags(null!).Count());
        }

        [Fact]
        public void Filters_CanBeChained()
        {
            var result = Items.GetItemsByTeam("real").GetItemsOnSale().GetItemsWithTags(new[] { "laLiga" });

            Assert.Equal(new[] { 4 }, Ids(result));
        }

        private static int[] Ids(IEnumerable<CatalogueItem> items)
        {
            return items.Select(i => i.TShirt.Id).ToArray();
        }

        private static CatalogueItem Item(int id, string team, Offer offer, int? discount, params string[] tags)
        {
            return new CatalogueItem(new Shirt(id, team, 50m, false, null, null, Size.M), offer, discount, tags);
        }
    }
}