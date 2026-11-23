namespace FTShirts.Web.Exceptions
{
	public class CatalogueItemNotFoundException : Exception
	{
		public int ShirtId { get; }

		public CatalogueItemNotFoundException(int shirtId)
			: base($"Couldn't find any item with ID: {shirtId}.")
		{
			ShirtId = shirtId;
		}

		public CatalogueItemNotFoundException(int shirtId, string message)
			: base(message)
		{
			ShirtId = shirtId;
		}

		public CatalogueItemNotFoundException(int shirtId, string message, Exception innerException)
			: base(message, innerException)
		{
			ShirtId = shirtId;
		}
	}
}