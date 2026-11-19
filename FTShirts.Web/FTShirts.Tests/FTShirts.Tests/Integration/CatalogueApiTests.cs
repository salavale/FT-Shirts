using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace FTShirts.Tests.Integration
{
    // Starts the real application (Program.cs) in memory and calls it over HTTP,
    // the same way the frontend does.
    public class CatalogueApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;

        public CatalogueApiTests(WebApplicationFactory<Program> factory)
        {
            _client = factory.CreateClient();
        }

        private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

        [Fact]
        public async Task GetCatalogue_ReturnsTheShirtsAsJson()
        {
            var items = await _client.GetFromJsonAsync<JsonElement>("/api/catalogue", Cancellation);

            Assert.True(items.GetArrayLength() >= 18);
            Assert.Contains("Boca Juniors", Teams(items));
        }

        [Fact]
        public async Task GetById_ExistingShirt_ReturnsIt()
        {
            var item = await _client.GetFromJsonAsync<JsonElement>("/api/catalogue/2", Cancellation);

            Assert.Equal("Boca Juniors", item.GetProperty("tShirt").GetProperty("team").GetString());
        }

        [Fact]
        public async Task GetById_UnknownShirt_Returns404()
        {
            var response = await _client.GetAsync("/api/catalogue/9999", Cancellation);

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task Search_ByTeam_ReturnsOnlyThatTeam()
        {
            var items = await _client.GetFromJsonAsync<JsonElement>("/api/catalogue/search?team=madrid", Cancellation);

            Assert.NotEmpty(Teams(items));
            Assert.All(Teams(items), team => Assert.Contains("Madrid", team));
        }

        [Fact]
        public async Task Search_OnlyOffers_ReturnsDiscountedShirts()
        {
            var items = await _client.GetFromJsonAsync<JsonElement>("/api/catalogue/search?onlyOffer=true", Cancellation);

            Assert.NotEqual(0, items.GetArrayLength());
            Assert.All(items.EnumerateArray(), item => Assert.NotEqual("No", item.GetProperty("offerType").GetString()));
        }

        [Fact]
        public async Task AddItem_NewShirt_AppearsInTheCatalogueAsSecondHand()
        {
            string team = $"Test FC {Guid.NewGuid():N}";
            var request = new
            {
                shirt = new { id = 0, team, price = 30.5m, isSecondHand = false, pNumber = (int?)null, pName = (string?)null, size = "M" },
                tags = new[] { "retro" }
            };

            var response = await _client.PostAsJsonAsync("/api/catalogue", request, Cancellation);
            var found = await _client.GetFromJsonAsync<JsonElement>($"/api/catalogue/search?team={Uri.EscapeDataString(team)}", Cancellation);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var item = Assert.Single(found.EnumerateArray());
            Assert.True(item.GetProperty("tShirt").GetProperty("isSecondHand").GetBoolean());
        }

        private static List<string?> Teams(JsonElement items)
        {
            return items.EnumerateArray()
                .Select(item => item.GetProperty("tShirt").GetProperty("team").GetString())
                .ToList();
        }
    }
}