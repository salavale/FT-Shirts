using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FTShirts.Tests.Integration
{
    public class UserApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        // The client keeps cookies between requests like a browser, so the
        // "UserLogged" cookie set by the login is sent with the next requests.
        private readonly HttpClient _client;
        private readonly string _username = $"user-{Guid.NewGuid():N}";

        public UserApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

        [Fact]
        public async Task Login_CorrectPassword_StartsASession()
        {
            var login = await Login("admin", "1234");
            var cart = await _client.GetFromJsonAsync<JsonElement>("/api/cart", Cancellation);

            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
            Assert.Equal("admin", cart.GetProperty("username").GetString());
        }

        [Fact]
        public async Task Login_WrongPassword_Returns401()
        {
            var login = await Login("admin", "wrong-password");

            Assert.Equal(HttpStatusCode.Unauthorized, login.StatusCode);
        }

        [Fact]
        public async Task Register_NewUser_CanLogInAfterwards()
        {
            var register = await Register(_username, "secret");
            var login = await Login(_username, "secret");

            Assert.Equal(HttpStatusCode.OK, register.StatusCode);
            Assert.Equal(HttpStatusCode.OK, login.StatusCode);
        }

        [Fact]
        public async Task Register_UsernameTaken_Returns409()
        {
            await Register(_username, "secret");

            var second = await Register(_username, "other-password");

            Assert.Equal(HttpStatusCode.Conflict, second.StatusCode);
        }

        [Fact]
        public async Task Logout_EndsTheSession()
        {
            await Login("admin", "1234");

            var logout = await _client.DeleteAsync("/api/user/logout", Cancellation);
            var cart = await _client.GetFromJsonAsync<JsonElement>("/api/cart", Cancellation);

            Assert.Equal(HttpStatusCode.OK, logout.StatusCode);
            Assert.Equal("dummy", cart.GetProperty("username").GetString());
        }

        private Task<HttpResponseMessage> Login(string username, string password)
        {
            return _client.PostAsJsonAsync("/api/user/login", new { username, password }, Cancellation);
        }

        private Task<HttpResponseMessage> Register(string username, string password)
        {
            return _client.PostAsJsonAsync("/api/user/register", new { username, password }, Cancellation);
        }
    }
}