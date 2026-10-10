using System.Collections.Generic;
namespace FTShirts.Web.Models
{
    public static class CatalogueExtensions
    {
        public static IEnumerable<CatalogueItem> GetItemsOnSale(this IEnumerable<CatalogueItem> catalogue)
        { 
            List<CatalogueItem> onSaleItems = new List<CatalogueItem>();

            foreach (var item in catalogue)
            { 
                if(item.OfferType == Offer.Yes)
                {
                    onSaleItems.Add(item);
                }
            }
            return onSaleItems;
        }
    }
}
