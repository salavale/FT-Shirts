using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FTShirts.Tests.Integration
{
    public class CartApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CartApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

        [Fact]
        public async Task AddToCart_LoggedInUser_ShirtAppearsInTheCart()
        {
            await LogInAsNewUser();

            var add = await _client.PostAsync("/api/cart/add/2", null, Cancellation);
            var cart = await _client.GetFromJsonAsync<JsonElement>("/api/cart", Cancellation);

            Assert.Equal(HttpStatusCode.OK, add.StatusCode);
            var item = Assert.Single(cart.GetProperty("shoppingCart").GetProperty("items").EnumerateArray());
            Assert.Equal(2, item.GetProperty("tShirt").GetProperty("id").GetInt32());
        }

        [Fact]
        public async Task GetCart_NotLoggedIn_ReturnsAnEmptyCart()
        {
            var cart = await _client.GetFromJsonAsync<JsonElement>("/api/cart", Cancellation);

            Assert.Equal(0, cart.GetProperty("shoppingCart").GetProperty("items").GetArrayLength());
        }

        private async Task LogInAsNewUser()
        {
            var user = new { username = $"user-{Guid.NewGuid():N}", password = "secret" };
            await _client.PostAsJsonAsync("/api/user/register", user, Cancellation);
            await _client.PostAsJsonAsync("/api/user/login", user, Cancellation);
        }
    }
}