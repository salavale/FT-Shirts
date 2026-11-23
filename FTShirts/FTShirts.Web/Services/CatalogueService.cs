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

        private List<CatalogueItem> LoadCatalogueItems()
        {
            return _context.CatalogueItems
                .Include(i => i.TShirt)
                .ToList();
        }

        public IEnumerable<CatalogueItem> GetAllItemsAsync()
        {
            return LoadCatalogueItems();
        }

        public IEnumerable<CatalogueItem> SearchItemsAsync(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged)
        {
            bool isMember = memberLogged == true;

            var query = _context.CatalogueItems
                .Include(i => i.TShirt)
                .ToOptionalQuery()
                .WhereIf(!string.IsNullOrWhiteSpace(team), item => item.TShirt.Team.ToLower().Contains(team!.ToLower()))
                .WhereIf(onlyOffer == true, item => item.OfferType == Offer.Yes || (isMember && item.OfferType == Offer.MemberOnly));

            foreach (string tag in tags ?? Enumerable.Empty<string>())
            {
                query = query.WhereIf(!string.IsNullOrWhiteSpace(tag), item => item.Tags.Contains(tag));
            }

            return query.ToListAsync().GetAwaiter().GetResult();
        }

        public CatalogueItem? GetItemByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Item ID must be greater than 0.");
            }
            var item = _context.CatalogueItems
                .Include(i => i.TShirt)
                .FirstOrDefault(i => i.ShirtId == id);

            if (item == null)
            {
                throw new CatalogueItemNotFoundException(id);
            }

            return item;
        }

        public void AddItemAsync(AddShirtRequest request)
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