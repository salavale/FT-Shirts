using FTShirts.Web.Models.UserModels;
using System.Collections.Concurrent;

namespace FTShirts.Web.Services
{
    // Registered as a singleton in Program.cs: one instance shared by every request.
    // That is why it keeps the users in a ConcurrentDictionary, which is thread-safe.
    public class ActiveUserStore : IActiveUserStore
    {
        private readonly ConcurrentDictionary<string, User> _users = new ConcurrentDictionary<string, User>();

        public User GetOrAdd(string username)
        {
            return _users.GetOrAdd(username, name => new User(name));
        }

        public User? Find(string username)
        {
            return _users.TryGetValue(username, out var user) ? user : null;
        }

        public void Remove(string username)
        {
            _users.TryRemove(username, out _);
        }
    }
}