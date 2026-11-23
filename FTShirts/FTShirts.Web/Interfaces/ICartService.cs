using FTShirts.Web.Models.UserModels;

namespace FTShirts.Web.Services
{
    public interface ICartService
    {
        User GetCartForUserAsync(string username);
        void AddItemToCartAsync(string username, int shirtId);
    }
}