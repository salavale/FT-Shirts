using FTShirts.Web.enums;
using System.Collections.Generic;
namespace FTShirts.Web.Models
{
    public static class CatalogueExtensions
    {
        public static IEnumerable<CatalogueItem> GetItemsOnSale(this IEnumerable<CatalogueItem> catalogue)
        {
            return catalogue.Where(item => item.OfferType == Offer.Yes);
        }
    
        public static IEnumerable<CatalogueItem> GetItemsOnSaleForMembers(this IEnumerable<CatalogueItem> catalogue)
        {
            return catalogue.Where(item => item.OfferType == Offer.Yes || item.OfferType == Offer.MemberOnly);
        }

        public static IEnumerable<CatalogueItem> GetItemsByTeam(this IEnumerable<CatalogueItem> catalogue, string team)
        {
            //If we introduce nothing, we return the whole catalogue
            if (string.IsNullOrWhiteSpace(team))
            {
                return catalogue;
            }

            // Team, tEAM, tEaM, tEAM must return the same team.
            return catalogue.Where(item => item.TShirt.Team.ToLower().Contains(team.ToLower()));
        }
        public static IEnumerable<CatalogueItem> GetItemsWithTags(this IEnumerable<CatalogueItem> catalogue, IEnumerable<string> tags)
        {
            foreach (var tag in tags)
            {
                if (!string.IsNullOrWhiteSpace(tag))
                {
                    catalogue = catalogue.Where(item => item.Tags.Contains(tag  ));
                }
            }

            return catalogue;
        }
    }
}
