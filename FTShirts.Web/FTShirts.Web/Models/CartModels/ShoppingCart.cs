using FTShirts.Web.Models.CatalogueModels;

namespace FTShirts.Web.Models.CartModels
{
    public class ShoppingCart
    {
        // The same cart can be changed by two requests at the same time (for example, the user
        // adds shirts from two tabs). List<T> is not thread-safe, so every access to the list
        // goes through this lock: only one thread at a time can be inside a lock (_lock) block.
        private readonly object _lock = new object();
        private readonly List<CatalogueItem> _items = new List<CatalogueItem>();

        // Returns a copy, so nobody can read the list while another request is changing it
        public IReadOnlyList<CatalogueItem> Items
        {
            get
            {
                lock (_lock)
                {
                    return _items.ToList();
                }
            }
        }

        public void AddItem(CatalogueItem item)
        {
            lock (_lock)
            {
                _items.Add(item);
            }
        }

        public void RemoveItem(int shirtId)
        {
            lock (_lock)
            {
                _items.Remove(_items.FirstOrDefault(i => i.TShirt.Id == shirtId));
            }
        }

        public decimal GetTotalPrice(bool member)
        {
            lock (_lock)
            {
                return _items.Sum(i => member ? i.MemberFinalPrice : i.FinalPrice);
            }
        }

        public void Clear()
        {
            lock (_lock)
            {
                _items.Clear();
            }
        }
    }
}