using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using FTShirts.Web.Services;

namespace FTShirts.Tests.Unit
{
    // Test double: a catalogue kept in memory, so CartService can be tested without the CSV file.
    internal class FakeCatalogueService : ICatalogueService
    {
        private readonly List<CatalogueItem> _items;

        public FakeCatalogueService(params CatalogueItem[] items)
        {
            _items = items.ToList();
        }

        public IEnumerable<CatalogueItem> GetAllItems()
        {
            return _items;
        }

        public IEnumerable<CatalogueItem> SearchItems(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged)
        {
            return _items;
        }

        public CatalogueItem? GetItemById(int id)
        {
            foreach (var item in _items)
            {
                if (item.TShirt.Id == id)
                {
                    return item;
                }
            }
            return null;
        }

        public void AddItem(AddShirtRequest request)
        {
            throw new NotSupportedException("Not needed by the tests.");
        }
    }
}