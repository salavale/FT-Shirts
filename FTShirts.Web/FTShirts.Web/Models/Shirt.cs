namespace FTShirts.Web.Models
{
    public enum SSize { S, M, L, XL, XXL }
    public class Shirt
    {
        //Identifying number of the shirt.
        public int Id { get; set; }
        //Team from which the shirt is from.
        public string Team { get; set; }
        //Price of the shirt.
        public decimal Price {  get; set; }
        //false: shirt is new.
        //true: shirt is second-handed.
        public bool IsSecondHand { get; set; }
        //number on the shirt back.
        //A shirt can have no number
        public int? PNumber { get; set; }
        //name on the shirt back.
        //A shirt can have no name.
        public string? PName { get; set; }
        //Size of the shirt.
        public SSize Size { get; set; }

    }
}
