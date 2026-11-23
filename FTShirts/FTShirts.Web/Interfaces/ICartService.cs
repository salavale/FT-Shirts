using FTShirts.Web.Models.UserModels;

namespace FTShirts.Web.Services
{
    public interface ICartService
    {
        Task<User> GetCartForUserAsync(string username);
        Task AddItemToCartAsync(string username, int shirtId);
    }
}