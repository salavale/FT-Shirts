using FTShirts.Web.enums;
using FTShirts.Web.Models.CatalogueModels;
using System.Collections.Generic;
namespace FTShirts.Web.Models.UserModels
{
    public class Member : User
    {
        //Void constructor create a dummy member
        public Member() : base("dummy member") { }
        public Member(string username) : base(username) { }

        //Override the User GetPrice so if an item have OnlyMember Offer, shows the correct price.
        public override decimal GetPrice(CatalogueItem item)
        {
            if (item.OfferType == Offer.MemberOnly) 
            {
                return item.MemberFinalPrice;
            }
            return base.GetPrice(item);
        }
    }
}
