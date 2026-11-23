using FTShirts.Web.Data;
using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using System.Linq;

namespace FTShirts.Web.Services
{
    public class UserService : IUserService
    {
        private readonly FTShirtsContext _ftshirts_context;

        public UserService(FTShirtsContext context)
        {
            _ftshirts_context = context;
        }

        public bool ValidateLogin(LogRegRequest request)
        {
            return _ftshirts_context.Users
                .Any(u => u.Username == request.Username && u.Password == request.Password);
        }

        public bool RegisterUser(LogRegRequest request)
        {
            if (_ftshirts_context.Users.Any(u => u.Username == request.Username))
            {
                return false;
            }

            var newUser = new User(request.Username, request.Password)
            {
                ShoppingCart = new ShoppingCart { Username = request.Username }
            };

            _ftshirts_context.Users.Add(newUser);
            _ftshirts_context.SaveChanges();
            return true;
        }

        public void EnsureUserActive(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return;
            }

            var user = _ftshirts_context.Users.FirstOrDefault(u => u.Username == username);
            if (user != null && _ftshirts_context.Set<ShoppingCart>().All(c => c.Username != username))
            {
                _ftshirts_context.Set<ShoppingCart>().Add(new ShoppingCart { Username = username });
                _ftshirts_context.SaveChanges();
            }
        }

        public void LogOutUpdate(string username)
        {
        }
    }
}