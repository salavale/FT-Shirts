using FTShirts.Web.Models.CatalogueModels;

namespace FTShirts.Web.Models.CartModels
{
    public class SCart
    {
        public List<CatalogueItem> Items { get; set; } = new List<CatalogueItem>();

        public void AddItem(CatalogueItem item)
        {
            Items.Add(item);
        }

        public void RemoveItem(int shirtId)
        {
            Items.Remove(Items.FirstOrDefault(i => i.TShirt.Id == shirtId));
        }

        public decimal GetTotalPrice(bool member)
        {
            return Items.Sum(i => member ? i.MemberFinalPrice : i.FinalPrice);
        }

        public void Clear()
        {
            Items.Clear();
        }
    }
}
