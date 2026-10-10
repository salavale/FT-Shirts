using System.Collections.Generic;
namespace FTShirts.Web.Models
{
    public static class CatalogueExtensions
    {
        public static IEnumerable<CatalogueItem> GetItemsOnSale(this IEnumerable<CatalogueItem> catalogue)
        {
            return catalogue.Where(item => item.OfferType == Offer.Yes);
        }
    }
}
