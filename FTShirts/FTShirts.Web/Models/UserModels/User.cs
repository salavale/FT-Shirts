using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.CatalogueModels;
using System.Collections.Generic;
namespace FTShirts.Web.Models.UserModels
{
    public class User
    {
        //Necessary credential to sign in to an specific account.
        public string Username { get; set; }
        public ShoppingCart ShoppingCart { get; set; }
        //Void constructor create a dummy member
        public User()
        {
            Username = "dummy";
            ShoppingCart = new ShoppingCart();
        }

        //Constructor. A member can sign up with a username and their pssword
        public User(string username)
        {
            Username = username;

            ShoppingCart = new ShoppingCart();
        }

        public virtual decimal GetPrice(CatalogueItem item)
        {
            return item.FinalPrice;
        }
    }
}
