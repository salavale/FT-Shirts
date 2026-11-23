using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using System.Collections.Generic;

namespace FTShirts.Web.Services
{
    public interface ICatalogueService
    {
        IEnumerable<CatalogueItem> GetAllItemsAsync();

        IEnumerable<CatalogueItem> SearchItemsAsync(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged);

        CatalogueItem? GetItemByIdAsync(int id);

        public void AddItemAsync(AddShirtRequest request);
    }
}