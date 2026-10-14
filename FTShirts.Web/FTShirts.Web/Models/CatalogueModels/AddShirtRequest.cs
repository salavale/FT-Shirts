namespace FTShirts.Web.Models.CatalogueModels
{
    public record AddShirtRequest(Shirt Shirt, IEnumerable<string>? Tags = null);
}
