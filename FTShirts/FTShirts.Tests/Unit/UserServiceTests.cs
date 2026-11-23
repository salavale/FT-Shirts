using FTShirts.Web.Data;
using FTShirts.Web.Models.CartModels;
using FTShirts.Web.Models.UserModels;
using FTShirts.Web.Models.UserModels.UserRequest;
using FTShirts.Web.Services;
using Microsoft.EntityFrameworkCore;

namespace FTShirts.Tests.Unit
{
    public class UserServiceTests : IDisposable
    {
        private readonly FTShirtsContext _context;
        private readonly UserService _userService;
        private readonly string _username = $"user-{Guid.NewGuid():N}";

        public UserServiceTests()
        {
            var options = new DbContextOptionsBuilder<FTShirtsContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new FTShirtsContext(options);
            _userService = new UserService(_context);
        }

        [Fact]
        public async Task ValidateLoginAsync_CorrectPassword_ReturnsTrue()
        {
            _context.Users.Add(new User("admin", "1234"));
            await _context.SaveChangesAsync();

            Assert.True(await _userService.ValidateLoginAsync(new LogRegRequest("admin", "1234")));
        }

        [Fact]
        public async Task ValidateLoginAsync_WrongPassword_ReturnsFalse()
        {
            _context.Users.Add(new User("admin", "1234"));
            await _context.SaveChangesAsync();

            Assert.False(await _userService.ValidateLoginAsync(new LogRegRequest("admin", "wrong-password")));
        }

        [Fact]
        public async Task ValidateLoginAsync_UnknownUser_ReturnsFalse()
        {
            Assert.False(await _userService.ValidateLoginAsync(new LogRegRequest(_username, "secret")));
        }

        [Fact]
        public async Task RegisterUserAsync_NewUsername_CanLogInAfterwards()
        {
            Assert.True(await _userService.RegisterUserAsync(new LogRegRequest(_username, "secret")));
            Assert.True(await _userService.ValidateLoginAsync(new LogRegRequest(_username, "secret")));
        }

        [Fact]
        public async Task RegisterUserAsync_UsernameTaken_ReturnsFalse()
        {
            _context.Users.Add(new User("admin", "1234"));
            await _context.SaveChangesAsync();

            Assert.False(await _userService.RegisterUserAsync(new LogRegRequest("admin", "other-password")));
        }

        [Fact]
        public void EnsureUserActive_ExistingUser_CreatesShoppingCartOnce()
        {
            _context.Users.Add(new User(_username, "secret"));
            _context.SaveChanges();

            _userService.EnsureUserActive(_username);
            _userService.EnsureUserActive(_username);

            int cartCount = _context.Set<ShoppingCart>().Count(c => c.Username == _username);
            Assert.Equal(1, cartCount);
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}