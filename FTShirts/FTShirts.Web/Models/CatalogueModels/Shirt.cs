using FTShirts.Web.enums;
using System.Globalization;
namespace FTShirts.Web.Models.CatalogueModels
{

    //This record is immutable.
    public record Shirt(int Id, string Team, decimal Price, bool IsSecondHand, int? PNumber, string? PName, Size Size)
    {
        public override string ToString()
        {
            return $"{Id},{Team},{Price.ToString(CultureInfo.InvariantCulture)},{IsSecondHand},{PNumber},{PName},{Size}";
        }
    }
}
