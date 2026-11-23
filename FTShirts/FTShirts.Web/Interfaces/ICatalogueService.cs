using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using FTShirts.Web.Models.CatalogueModels.CatalogueRequest;
using System.Collections.Generic;

namespace FTShirts.Web.Services
{
    public interface ICatalogueService
    {
        IEnumerable<CatalogueItem> GetAllItems();

        IEnumerable<CatalogueItem> SearchItems(string? team, bool? onlyOffer, IEnumerable<string>? tags, bool? memberLogged);

        CatalogueItem? GetItemById(int id);

        void AddItem(AddShirtRequest request);
    }
}