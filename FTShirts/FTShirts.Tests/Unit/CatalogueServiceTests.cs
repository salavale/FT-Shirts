using FTShirts.Web.Data;
using FTShirts.Web.enums;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using FTShirts.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FTShirts.Tests.Unit
{
    public class CatalogueServiceTests : IDisposable
    {
        private readonly FTShirtsContext _context;
        private readonly CatalogueService _catalogueService;

        public CatalogueServiceTests()
        {
            var options = new DbContextOptionsBuilder<FTShirtsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FTShirtsContext(options);
            _catalogueService = new CatalogueService(_context);
        }

        [Fact]
        public void GetAllItems_ReturnsSeededCatalogueItems()
        {
            SeedItem(1, "Boca Juniors", 40m, Offer.No, null, new[] { "classic" });

            var items = _catalogueService.GetAllItems().ToList();

            Assert.Contains(items, i => i.TShirt != null && i.TShirt.Team == "Boca Juniors");
        }

        [Fact]
        public void GetItemById_ReturnsTheItemOrNull()
        {
            var seeded = SeedItem(2, "Boca Juniors", 40m, Offer.No, null, Array.Empty<string>());

            var found = _catalogueService.GetItemById(seeded.TShirt!.Id);

            Assert.NotNull(found);
            Assert.Equal("Boca Juniors", found.TShirt!.Team);
            Assert.Null(_catalogueService.GetItemById(-1));
        }

        [Fact]
        public void SearchItems_ByTeamAndTag_FiltersTheCatalogue()
        {
            SeedItem(1, "Real Madrid", 55m, Offer.No, null, new[] { "retro", "home" });
            SeedItem(2, "Real Madrid", 60m, Offer.No, null, new[] { "away" });
            SeedItem(3, "Barcelona", 50m, Offer.No, null, new[] { "retro" });

            var results = _catalogueService
                .SearchItems("real madrid", onlyOffer: false, tags: new[] { "retro" }, memberLogged: false)
                .ToList();

            Assert.Single(results);
            var item = results[0];
            Assert.Equal("Real Madrid", item.TShirt!.Team);
            Assert.Contains("retro", item.Tags);
        }

        [Fact]
        public void SearchItems_OnlyOffers_ReturnsDiscountedShirts()
        {
            SeedItem(1, "River Plate", 45m, Offer.Yes, 15, Array.Empty<string>());
            SeedItem(2, "Boca Juniors", 40m, Offer.No, null, Array.Empty<string>());

            var results = _catalogueService
                .SearchItems(null, onlyOffer: true, tags: null, memberLogged: false)
                .ToList();

            Assert.Single(results);
            Assert.NotEqual(Offer.No, results[0].OfferType);
        }

        [Fact]
        public void AddItem_SavesTheShirtAsSecondHandInContext()
        {
            string team = $"Test FC {Guid.NewGuid():N}";
            var shirt = new Shirt(0, team, 25m, false, null, null, Size.L);
            var request = new AddShirtRequest(shirt, new[] { "retro" });

            _catalogueService.AddItem(request);

            var saved = Assert.Single(_catalogueService.GetAllItems(), i => i.TShirt != null && i.TShirt.Team == team);
            Assert.NotNull(saved.TShirt);
            Assert.True(saved.TShirt.IsSecondHand);
            Assert.True(saved.TShirt.Id > 0);
        }

        private CatalogueItem SeedItem(int id, string team, decimal price, Offer offer, int? discount, string[] tags)
        {
            var shirt = new Shirt(id, team, price, false, null, null, Size.M);
            var item = new CatalogueItem(shirt, offer, discount)
            {
                Tags = tags.ToList()
            };

            _context.Add(shirt);
            _context.Add(item);
            _context.SaveChanges();

            return item;
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}