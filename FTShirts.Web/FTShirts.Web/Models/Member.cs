using System.Collections.Generic;
namespace FTShirts.Web.Models
{
    public class Member
    {
        //Necessary credential to sign in to an specific account.
        public string Username { get; set; }
        public string Password { get; set; }
        public List<CatalogueItem> ShoppingCart { get; set; }
        //Void constructor create a dummy member
        public Member()
        {
            Username = "dummy";
            Password = "";

            ShoppingCart = new List<CatalogueItem>();
        }

        //Constructor. A member can sign up with a username and their pssword
        public Member ( string username, string password )
        {
            Username = username;
            Password = password;

            ShoppingCart = new List<CatalogueItem> ();
        }
        //This constructor let us save the member on a csv file 
        //and have thir credential store. We follow the same logic as 
        //in the CatalogueItem constructor.
        public Member(string csvLine)
        {
            string[] data = csvLine.Split(',');

            Username = data[0];
            Password = data[1];

            ShoppingCart = new List<CatalogueItem>();
        }
    }
}
