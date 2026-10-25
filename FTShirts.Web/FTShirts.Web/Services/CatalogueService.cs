using FTShirts.Web.enums;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class CatalogueService : ICatalogueService
    {
        private static Catalogue _storeCatalogue = new Catalogue();
        private const string FilePath = "./Data/shirts.csv";

        public CatalogueService()
        {
            if (File.Exists(FilePath) && !_storeCatalogue.Any())
            {
                foreach (var line in File.ReadLines(FilePath).Where(l => !string.IsNullOrWhiteSpace(l)))
                {
                    _storeCatalogue.AddItem(new CatalogueItem(line));
                }
            }
        }

        public IEnumerable<CatalogueItem> GetAllItems()
        {
            return _storeCatalogue;
        }

        public IEnumerable<CatalogueItem> SearchItems(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged)
        {
            var results = _storeCatalogue.GetItemsByTeam(team: team);

            if (onlyOffer.HasValue && onlyOffer.Value)
            {
                if (memberLogged.Value)
                {
                    results = results.GetItemsOnSale();
                }
                else
                {
                    results = results.GetItemsOnSaleForMembers();
                }
            }

            results = results.GetItemsWithTags(tags);

            return results;
        }

        public CatalogueItem? GetItemById(int id)
        {
            return _storeCatalogue.GetItemByID(id);
        }

        public void AddItem(AddShirtRequest request)
        {
            int newId = _storeCatalogue.Any() ? _storeCatalogue.Max(i => i.TShirt.Id) + 1 : 1;
            Shirt finalShirt = request.Shirt with { Id = newId, IsSecondHand = true };

            var newItem = new CatalogueItem(shirt: finalShirt, offerType: Offer.No, tags: request.Tags);

            File.AppendAllText(FilePath, newItem.ToString() + "\n");
            _storeCatalogue.AddItem(newItem);
        }
    }
}