using FTShirts.Web.Models.UserModels;

namespace FTShirts.Web.Services
{
    public interface ICartService
    {
        User GetCartForUser(string username);
        void AddItemToCart(string username, int shirtId);
    }
}