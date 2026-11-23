using FTShirts.Web.enums;
using System.Collections;

namespace FTShirts.Web.Models.CatalogueModels
{
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
