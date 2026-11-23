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

        // Every query to the database is IO, so it is done asynchronously:
        // while the database answers, the thread is free to serve other requests.
        private async Task<List<CatalogueItem>> LoadCatalogueItemsAsync()
        {
            return await _context.CatalogueItems
                .Include(i => i.TShirt)
                .ToListAsync();
        }

        public async Task<IEnumerable<CatalogueItem>> GetAllItemsAsync()
        {
            return await LoadCatalogueItemsAsync();
        }

        public async Task<IEnumerable<CatalogueItem>> SearchItemsAsync(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged)
        {
            bool isMember = memberLogged == true;

            // Every filter is only used if the user filled it in,
            // and the whole search runs in the database instead of loading every shirt
            var query = _context.CatalogueItems
                .Include(i => i.TShirt)
                .ToOptionalQuery()
                // Team: part of the name, without caring about upper or lower case
                .WhereIf(!string.IsNullOrWhiteSpace(team), item => item.TShirt.Team.ToLower().Contains(team!.ToLower()))
                // Offers: members also see the offers that are only for members
                .WhereIf(onlyOffer == true, item => item.OfferType == Offer.Yes || (isMember && item.OfferType == Offer.MemberOnly));

            // Tags: the shirt must have every selected tag
            foreach (string tag in tags ?? Enumerable.Empty<string>())
            {
                query = query.WhereIf(!string.IsNullOrWhiteSpace(tag), item => item.Tags.Contains(tag));
            }

            return await query.ToListAsync();
        }

        public async Task<CatalogueItem?> GetItemByIdAsync(int id)
        {
            return await _context.CatalogueItems
                .Include(i => i.TShirt)
                .FirstOrDefaultAsync(i => i.ShirtId == id);
        }

        public async Task AddItemAsync(AddShirtRequest request)
        {
            int maxId = await _context.Shirts.AnyAsync() ? await _context.Shirts.MaxAsync(s => s.Id) : 0;
            int newId = maxId + 1;

            Shirt finalShirt = request.Shirt with { Id = newId, IsSecondHand = true };

            var newItem = new CatalogueItem(shirt: finalShirt, offerType: Offer.No, tags: request.Tags)
            {
                ShirtId = newId
            };

            _context.Shirts.Add(finalShirt);
            _context.CatalogueItems.Add(newItem);
            await _context.SaveChangesAsync();
        }
    }
}