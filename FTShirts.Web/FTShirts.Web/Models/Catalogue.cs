using System.Collections; //Necessary for IEnumerable

namespace FTShirts.Web.Models
{
    //We will like to have an offer feature, where an item from
    //the catalogue can have a discount, none or only for the members.
    public enum Offer { Yes, No, MemberOnly }
    //Our list on the catalogue will be the shirt, if it has any offer and the amount is discounted
    public struct CatalogueItem
    {
        public Shirt TShirt { get; set; }
        public Offer OfferType { get; set; }
        public int? OfferDiscount { get; set; }
        public List<string> Tags { get; }
        public decimal FinalPrice
        {
            get
            {
                if (OfferType == Offer.Yes)
                {
                    decimal discountMultiplier = 1m - (OfferDiscount.Value / 100m);
                    return TShirt.Price * discountMultiplier;
                }
                return TShirt.Price;
            }
        }
        public decimal MemberFinalPrice
        {
            get
            {
                if (OfferType == Offer.MemberOnly || OfferType == Offer.Yes)
                {
                    decimal discountMultiplier = 1m - (OfferDiscount.Value / 100m);
                    return TShirt.Price * discountMultiplier;
                }
                return TShirt.Price;
            }
        }
        //constructor
        public CatalogueItem(Shirt shirt, Offer offerType, int? discount = null)
        {
            TShirt = shirt;
            OfferType = offerType;
            OfferDiscount = discount;
            Tags = new List<string>();
        }
        //Constructor to read from a csv file the data of a ItemCatalogue.
        //The format must be:
        // id, team, price, isSecondHand, pNumber, pName, size, offer, discount
        //Where the seven first elements define a shirt and the other 2 the offertype and offerdiscount.
        //this constructor was made by the LLM used in the project.
        public CatalogueItem(string csvLine)
        {
            Tags = new List<string>();
            string[] data = csvLine.Split(',');

            int id = int.Parse(data[0]);
            string team = data[1];
            decimal price = decimal.Parse(data[2], System.Globalization.CultureInfo.InvariantCulture);
            bool isSecondHand = bool.Parse(data[3]);
            int? pNumber = string.IsNullOrEmpty(data[4]) ? null : int.Parse(data[4]);
            string? pName = string.IsNullOrEmpty(data[5]) ? null : data[5];
            SSize size = Enum.Parse<SSize>(data[6]);

            TShirt = new Shirt(id, team, price, isSecondHand, pNumber, pName, size);

            OfferType = Enum.Parse<Offer>(data[7]);
            OfferDiscount = string.IsNullOrEmpty(data[8]) ? null : int.Parse(data[8]);
            for (var i = 9; i < data.Length; i++)
            {
                if (!string.IsNullOrEmpty(data[i]))
                {
                    Tags.Add(data[i].Trim());
                }
                
            }
        }
    }
    public class Catalogue : IEnumerable<CatalogueItem>
    {
        //We can consider making it private readonly
        //to avoid overwrite the list.
        private readonly List<CatalogueItem> _items;

        //Void constructor
        public Catalogue() {
            _items = new List<CatalogueItem>();
        }
        //Constructor with a CatalogueItem Ienumerable
        public Catalogue(IEnumerable<CatalogueItem> items)
        {
            _items = new List<CatalogueItem>(items);
        }
        //Constructor with a Shirt Ienumerable
        public Catalogue(IEnumerable<Shirt> shirts)
        {
            _items = new List<CatalogueItem>();

            foreach (Shirt shirt in shirts) 
            {
                AddShirt(shirt);
            }
        }
        public void AddItem(CatalogueItem item)
        {
            _items.Add(item);
        }
        public CatalogueItem? GetItem(int index)
        {
            //if index if OOB we return null
            if (index < 0 || index >= _items.Count)
            {
                return null;
            }
            return _items[index];
        }
        public CatalogueItem? GetItemByID(int id)
        {
            foreach (var item in _items)
            {
                if (item.TShirt.Id == id) return item;
            }
            //if there is no shirt with Id=id
            return null;
        }
        public void AddShirt(Shirt shirt) 
        {
            CatalogueItem item = new CatalogueItem( shirt, Offer.No);
            _items.Add(item);
        }
        public Shirt? GetShirt (int index) 
        { 
            //if index if OOB we return null
            if( index < 0 || index>= _items.Count)
            {
                return null;
            }
            return _items[index].TShirt;
        }
        public Shirt? GetShirtByID(int id)
        {
            foreach (var item in _items)
            {
                if (item.TShirt.Id == id) return item.TShirt;
            }
            //if there is no shirt with Id=id
            return null;
        }
        public IEnumerator<CatalogueItem> GetEnumerator() 
        {
            //like a pointer to the first element of _shirts
            return _items.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
