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
            // The items are loaded from the database asynchronously,
            // then filtered in memory with the extension methods
            var results = (await LoadCatalogueItemsAsync()).GetItemsByTeam(team: team);

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