using FTShirts.Web.Models.UserModels;

namespace FTShirts.Web.Services
{
    // The users that are logged in, with their shopping carts.
    public interface IActiveUserStore
    {
        // Returns the active user with that name, adding it first if it is not there yet
        User GetOrAdd(string username);

        // Returns the active user with that name, or null if that user is not logged in
        User? Find(string username);

        void Remove(string username);
    }
}