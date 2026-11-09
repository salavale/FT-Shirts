namespace FTShirts.Web.Models.CatalogueModels.CatalogueRequest
{
    public record AddShirtRequest(Shirt Shirt, IEnumerable<string>? Tags = null);
}
