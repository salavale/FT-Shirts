using FTShirts.Web.Data;
using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using Microsoft.EntityFrameworkCore;

namespace FTShirts.Web.Services
{
    public class UserService : IUserService
    {
        private readonly FTShirtsContext _context;
        private static readonly object _activeUsersLock = new object();
        private static readonly HashSet<string> _activeUsernames = new HashSet<string>();

        public UserService(FTShirtsContext context)
        {
            _context = context;
        }

        public async Task<bool> ValidateLoginAsync(LogRegRequest request)
        {
            return await _context.Users
                .AnyAsync(u => u.Username == request.Username && u.Password == request.Password);
        }

        public async Task<bool> RegisterUserAsync(LogRegRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username))
            {
                return false;
            }

            var newUser = new User(request.Username, request.Password)
            {
                ShoppingCart = new ShoppingCart { Username = request.Username }
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return true;
        }

        public void EnsureUserActive(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            var user = _context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null && _context.Set<ShoppingCart>().All(c => c.Username != username))
            {
                _context.Set<ShoppingCart>().Add(new ShoppingCart { Username = username });
                _context.SaveChanges();
            }

            if (user != null)
            {
                lock (_activeUsersLock)
                {
                    _activeUsernames.Add(username);
                }
            }
        }

        public void LogOutUpdate(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            lock (_activeUsersLock)
            {
                _activeUsernames.Remove(username);
            }
        }

        public int GetActiveUsersCount()
        {
            lock (_activeUsersLock)
            {
                return _activeUsernames.Count;
            }
        }   
    }
}