using FTShirts.Web.enums;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using FTShirts.Web.Services;

namespace FTShirts.Tests.Unit
{
    // Uses the copy of shirts.csv prepared by TestDataFixture, never the real file.
    public class CatalogueServiceTests
    {
        private readonly CatalogueService _catalogueService = new CatalogueService();

        [Fact]
        public void GetAllItems_LoadsTheShirtsFromTheCsvFile()
        {
            Assert.Contains(_catalogueService.GetAllItems(), i => i.TShirt.Team == "Boca Juniors");
        }

        [Fact]
        public void GetItemById_ReturnsTheItemOrNull()
        {
            Assert.Equal("Boca Juniors", _catalogueService.GetItemById(2)?.TShirt.Team);
            Assert.Null(_catalogueService.GetItemById(-1));
        }

        [Fact]
        public void SearchItems_ByTeamAndTag_FiltersTheCatalogue()
        {
            var results = _catalogueService.SearchItems("real madrid", onlyOffer: false, tags: new[] { "retro" }, memberLogged: false).ToList();

            Assert.NotEmpty(results);
            Assert.All(results, item =>
            {
                Assert.Equal("Real Madrid", item.TShirt.Team);
                Assert.Contains("retro", item.Tags);
            });
        }

        [Fact]
        public void SearchItems_OnlyOffers_ReturnsDiscountedShirts()
        {
            var results = _catalogueService.SearchItems(null, onlyOffer: true, tags: null, memberLogged: false).ToList();

            Assert.NotEmpty(results);
            Assert.All(results, item => Assert.NotEqual(Offer.No, item.OfferType));
        }

        [Fact]
        public void AddItem_SavesTheShirtAsSecondHandWithANewId()
        {
            string team = $"Test FC {Guid.NewGuid():N}";
            var request = new AddShirtRequest(new Shirt(0, team, 25m, false, null, null, Size.L), new[] { "retro" });

            _catalogueService.AddItem(request);

            var saved = Assert.Single(_catalogueService.GetAllItems(), i => i.TShirt.Team == team);
            Assert.True(saved.TShirt.IsSecondHand);
            Assert.True(saved.TShirt.Id > 18);
            Assert.Contains(team, File.ReadAllText("./Data/shirts.csv"));
        }
    }
}