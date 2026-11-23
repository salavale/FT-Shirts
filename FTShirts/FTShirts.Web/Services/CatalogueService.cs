using FTShirts.Web.Data;
using FTShirts.Web.enums;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class CatalogueService : ICatalogueService
    {
        private readonly FTShirtsContext _context;

        public CatalogueService(FTShirtsContext context)
        {
            _context = context;
        }

        private IEnumerable<CatalogueItem> LoadCatalogueItems()
        {
            return _context.CatalogueItems
                .Include(i => i.TShirt)
                .AsEnumerable();
        }

        public IEnumerable<CatalogueItem> GetAllItems()
        {
            return LoadCatalogueItems().ToList();
        }

        public IEnumerable<CatalogueItem> SearchItems(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged)
        {
            var results = LoadCatalogueItems().GetItemsByTeam(team: team);

            if (onlyOffer.HasValue && onlyOffer.Value)
            {
                if (memberLogged == true)
                {
                    results = results.GetItemsOnSaleForMembers();
                }
                else
                {
                    results = results.GetItemsOnSale();
                }
            }

            results = results.GetItemsWithTags(tags);

            return results.ToList();
        }

        public CatalogueItem? GetItemById(int id)
        {
            return _context.CatalogueItems
                .Include(i => i.TShirt)
                .FirstOrDefault(i => i.ShirtId == id);
        }

        public void AddItem(AddShirtRequest request)
        {
            int maxId = _context.Shirts.Any() ? _context.Shirts.Max(s => s.Id) : 0;
            int newId = maxId + 1;

            Shirt finalShirt = request.Shirt with { Id = newId, IsSecondHand = true };

            var newItem = new CatalogueItem(shirt: finalShirt, offerType: Offer.No, tags: request.Tags)
            {
                ShirtId = newId
            };

            _context.Shirts.Add(finalShirt);
            _context.CatalogueItems.Add(newItem);
            _context.SaveChanges();
        }
    }
}