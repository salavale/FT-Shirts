using FTShirts.Web.enums;
namespace FTShirts.Web.Models.CatalogueModels
{

    //This record is immutable.
    public record Shirt(int Id, string Team, decimal Price, bool IsSecondHand, int? PNumber, string? PName, Size Size)
    {
        public override string ToString()
        {
            return $"{Id},{Team},{Price},{IsSecondHand},{PNumber},{PName},{Size}";
        }
    }
}
