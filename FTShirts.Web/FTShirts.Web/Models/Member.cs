using System.Collections.Generic;
namespace FTShirts.Web.Models
{
    public class Member
    {
        //Necessary credential to sign in to an specific account.
        public string Username { get; set; }
        public List<CatalogueItem> ShoppingCart { get; set; }
        //Void constructor create a dummy member
        public Member()
        {
            Username = "dummy";
            ShoppingCart = new List<CatalogueItem>();
        }

        //Constructor. A member can sign up with a username and their pssword
        public Member ( string username )
        {
            Username = username;

            ShoppingCart = new List<CatalogueItem> ();
        }
    }
}
