using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using System.Collections.Generic;

namespace FTShirts.Web.Services
{
    public interface ICatalogueService
    {
        Task<IEnumerable<CatalogueItem>> GetAllItemsAsync();

        Task<IEnumerable<CatalogueItem>> SearchItemsAsync(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged);

        Task<CatalogueItem?> GetItemByIdAsync(int id);

        Task AddItemAsync(AddShirtRequest request);
    }
}