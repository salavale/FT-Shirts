using FTShirts.Web.enums;

namespace FTShirts.Web.Models.CatalogueModels
{
    //Our list on the catalogue will be the shirt, if it has any offer and the amount is discounted
    public struct CatalogueItem
    {
        public Shirt TShirt { get; set; }
        public Offer OfferType { get; set; }
        public int? OfferDiscount { get; set; }
        public List<string> Tags { get; }
        //constructor
        public CatalogueItem(Shirt shirt, Offer offerType, int? discount = null, IEnumerable<string>? tags = null)
        {
            TShirt = shirt;
            OfferType = offerType;
            OfferDiscount = discount;
            Tags = new List<string>();
            if(tags != null)
            {
                foreach (string tag in tags)
                {
                    Tags.Add(tag);
                }
            }
        }
        //Constructor to read from a csv file the data of a ItemCatalogue.
        //The format must be:
        // id, team, price, isSecondHand, [pNumber], [pName], size, offer, [discount], [tag1, tag2, ...]
        //Where the seven first elements define a shirt and the next two the offertype and offerdiscount and the rest the different tags
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
            Size size = Enum.Parse<Size>(data[6]);

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

        public void AddTag( string tag)
        {
            Tags.Add(tag);
        }

        public override string ToString()
        {
            return $"{TShirt.ToString()},{OfferType},{OfferDiscount},{string.Join(",", Tags)}";
        }
    }
}
