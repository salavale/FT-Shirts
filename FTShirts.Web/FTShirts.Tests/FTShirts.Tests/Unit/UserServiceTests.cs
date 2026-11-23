using FTShirts.Web.Models.UserModels.UserRequest;
using FTShirts.Web.Services;

namespace FTShirts.Tests.Unit
{
    // Uses the copy of users.csv prepared by TestDataFixture, never the real file.
    public class UserServiceTests
    {
        // Each test gets its own store of logged-in users, injected into the service
        private readonly ActiveUserStore _activeUsers = new ActiveUserStore();
        private readonly UserService _userService;
        private readonly string _username = $"user-{Guid.NewGuid():N}";

        public UserServiceTests()
        {
            _userService = new UserService(_activeUsers);
        }

        [Fact]
        public async Task ValidateLoginAsync_CorrectPassword_ReturnsTrue()
        {
            Assert.True(await _userService.ValidateLoginAsync(new LogRegRequest("admin", "1234")));
        }

        [Fact]
        public async Task ValidateLoginAsync_WrongPassword_ReturnsFalse()
        {
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
            Assert.False(await _userService.RegisterUserAsync(new LogRegRequest("admin", "other-password")));
        }

        [Fact]
        public void EnsureUserActive_ThenLogOut_AddsAndRemovesTheActiveUser()
        {
            _userService.EnsureUserActive(_username);
            Assert.NotNull(_activeUsers.Find(_username));

            _userService.LogOutUpdate(_username);
            Assert.Null(_activeUsers.Find(_username));
        }

        [Fact]
        public void EnsureUserActive_CalledTwice_KeepsTheSameUserAndCart()
        {
            _userService.EnsureUserActive(_username);
            var first = _activeUsers.Find(_username);

            _userService.EnsureUserActive(_username);

            Assert.Same(first, _activeUsers.Find(_username));
        }

        [Fact]
        public void LogOutUpdate_NoUsername_DoesNothing()
        {
            var exception = Record.Exception(() => _userService.LogOutUpdate(null!));

            Assert.Null(exception);
        }
    }
}