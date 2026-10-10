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
                if (OfferType == Offer.MemberOnly)
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
