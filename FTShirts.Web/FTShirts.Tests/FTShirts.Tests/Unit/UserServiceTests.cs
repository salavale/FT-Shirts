using FTShirts.Web.Models.UserModels.UserRequest;
using FTShirts.Web.Services;

namespace FTShirts.Tests.Unit
{
    // Uses the copy of users.csv prepared by TestDataFixture, never the real file.
    public class UserServiceTests
    {
        private readonly UserService _userService = new UserService();
        private readonly string _username = $"user-{Guid.NewGuid():N}";

        [Fact]
        public void ValidateLogin_CorrectPassword_ReturnsTrue()
        {
            Assert.True(_userService.ValidateLogin(new LogRegRequest("admin", "1234")));
        }

        [Fact]
        public void ValidateLogin_WrongPassword_ReturnsFalse()
        {
            Assert.False(_userService.ValidateLogin(new LogRegRequest("admin", "wrong-password")));
        }

        [Fact]
        public void RegisterUser_NewUsername_CanLogInAfterwards()
        {
            Assert.True(_userService.RegisterUser(new LogRegRequest(_username, "secret")));
            Assert.True(_userService.ValidateLogin(new LogRegRequest(_username, "secret")));
        }

        [Fact]
        public void RegisterUser_UsernameTaken_ReturnsFalse()
        {
            Assert.False(_userService.RegisterUser(new LogRegRequest("admin", "other-password")));
        }

        [Fact]
        public void EnsureUserActive_ThenLogOut_AddsAndRemovesTheActiveUser()
        {
            _userService.EnsureUserActive(_username);
            Assert.True(UserService.ActiveUsers.ContainsKey(_username));

            _userService.LogOutUpdate(_username);
            Assert.False(UserService.ActiveUsers.ContainsKey(_username));
        }

        [Fact]
        public void EnsureUserActive_CalledTwice_KeepsTheSameUserAndCart()
        {
            _userService.EnsureUserActive(_username);
            var first = UserService.ActiveUsers[_username];

            _userService.EnsureUserActive(_username);

            Assert.Same(first, UserService.ActiveUsers[_username]);
        }

        [Fact]
        public void LogOutUpdate_NoUsername_DoesNothing()
        {
            var exception = Record.Exception(() => _userService.LogOutUpdate(null!));

            Assert.Null(exception);
        }
    }
}