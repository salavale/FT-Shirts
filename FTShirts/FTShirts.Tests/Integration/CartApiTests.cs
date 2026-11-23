using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FTShirts.Web.Data;
using FTShirts.Web.enums;
using FTShirts.Web.Models;
using FTShirts.Web.Models.CatalogueModels;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FTShirts.Tests.Integration
{
    public class CartApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public CartApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private static CancellationToken Cancellation => TestContext.Current.CancellationToken;

        [Fact]
        public async Task AddToCart_LoggedInUser_ShirtAppearsInTheCart()
        {
            var client = _factory.CreateClient();
            int shirtId = await EnsureTestCatalogueItemAsync(Cancellation);
            string username = $"testuser_{Guid.NewGuid():N}";

            try
            {
                await LogInAsNewUserAsync(client, username, Cancellation);

                var add = await client.PostAsync($"/api/cart/add/{shirtId}", null, Cancellation);
                Assert.Equal(HttpStatusCode.OK, add.StatusCode);

                var cart = await client.GetFromJsonAsync<JsonElement>("/api/cart", Cancellation);
                var items = cart.GetProperty("shoppingCart").GetProperty("items").EnumerateArray().ToList();
                var item = Assert.Single(items);
                Assert.Equal(shirtId, item.GetProperty("tShirt").GetProperty("id").GetInt32());
            }
            finally
            {
                await CleanupUserAsync(username, Cancellation);
            }
        }

        [Fact]
        public async Task GetCart_NotLoggedIn_ReturnsAnEmptyCart()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            });

            var cart = await client.GetFromJsonAsync<JsonElement>("/api/cart", Cancellation);
            var items = cart.GetProperty("shoppingCart").GetProperty("items");

            Assert.Equal(JsonValueKind.Array, items.ValueKind);
            Assert.Empty(items.EnumerateArray());
        }

        private static async Task LogInAsNewUserAsync(HttpClient client, string username, CancellationToken ct)
        {
            var user = new { username, password = "secret" };
            var registerResponse = await client.PostAsJsonAsync("/api/user/register", user, ct);
            registerResponse.EnsureSuccessStatusCode();

            var loginResponse = await client.PostAsJsonAsync("/api/user/login", user, ct);
            loginResponse.EnsureSuccessStatusCode();
        }

        private async Task<int> EnsureTestCatalogueItemAsync(CancellationToken ct)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FTShirtsContext>();

            var existing = await db.CatalogueItems
                .Include(c => c.TShirt)
                .FirstOrDefaultAsync(ct);

            if (existing != null)
            {
                return existing.ShirtId;
            }

            var shirt = new Shirt(0, "Test FC", 29.99m, false, null, null, Size.M)
            {
                Team = "Test FC",
                Price = 29.99m,
                IsSecondHand = false
            };

            db.Shirts.Add(shirt);
            await db.SaveChangesAsync(ct);

            var catalogueItem = new CatalogueItem(shirt, Offer.No, null)
            {
                ShirtId = shirt.Id
            };

            db.CatalogueItems.Add(catalogueItem);
            await db.SaveChangesAsync(ct);

            return shirt.Id;
        }

        private async Task CleanupUserAsync(string username, CancellationToken ct)
        {
            using var scope = _factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<FTShirtsContext>();

            var user = await db.Users
                .Include(u => u.ShoppingCart)
                .ThenInclude(c => c!.Items)
                .FirstOrDefaultAsync(u => u.Username == username, ct);

            if (user != null)
            {
                if (user.ShoppingCart != null)
                {
                    db.RemoveRange(user.ShoppingCart.Items);
                    db.Remove(user.ShoppingCart);
                }

                db.Users.Remove(user);
            }

            await db.SaveChangesAsync(ct);
        }
    }
}