using System.Net;
using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;

namespace EcommerceApi.IntegrationTests
{
    public class CartControllerIntegrationTests : BaseIntegrationTest
    {
        private User _testUser;
        private User _otherUser;
        private Product _product1;
        private Product _product2;
        private Product _product3;

        public CartControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        protected override async Task SeedSpecificTestData(ApplicationDbContext context)
        {
            // Add users
            _testUser = new User
            {
                Id = "test-user-123",
                UserName = "testUser",
                Email = "test@example.com",
                // Since we are using context.Users and not directly UserManager, we need to set these
                NormalizedEmail = "TEST@EXAMPLE.COM",
                NormalizedUserName = "TESTUSER"
            };

            _otherUser = new User
            {
                Id = "other-user-456",
                UserName = "otherUser",
                Email = "other@example.com",
                NormalizedEmail = "OTHER@EXAMPLE.COM",
                NormalizedUserName = "OTHERUSER"
            };
            context.Users.AddRange(_testUser, _otherUser);

            // Add products
            _product1 = new Product
            {
                Id = 1,
                Sku = "SKU001",
                Name = "Test Product 1",
                Description = "Description for product 1",
                Price = 5m,
                ImageUrl = "https://example.com/image1.jpg"
            };

            _product2 = new Product
            {
                Id = 2,
                Sku = "SKU002",
                Name = "Test Product 2",
                Description = "Description for product 2",
                Price = 10m
            };

            _product3 = new Product
            {
                Id = 3,
                Sku = "SKU003",
                Name = "Test Product 3",
                Description = "Description for product 3",
                Price = 15m
            };

            context.Products.AddRange(_product1, _product2, _product3);

            // Add cart for test user with items
            var testUserCart = new Cart
            {
                Id = 1,
                UserId = _testUser.Id,
                Items = new List<CartItem>
                {
                    new CartItem
                    {
                        Id = 1,
                        CartId = 1,
                        ProductId = 1,
                        Quantity = 2
                    },
                    new CartItem
                    {
                        Id = 2,
                        CartId = 1,
                        ProductId = 2,
                        Quantity = 1
                    }
                }
            };

            // Add empty cart for other user
            var otherUserCart = new Cart
            {
                Id = 2,
                UserId = _otherUser.Id,
                Items = new List<CartItem>()
            };
            context.Carts.AddRange(testUserCart, otherUserCart);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetCart_WithExistingCart_ReturnsOkCartData()
        {
            // using var scope = Factory.Services.CreateScope();
            // var provider = scope.ServiceProvider.GetRequiredService<ITestUserProvider>();
            // provider.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(
            // [
            //     new Claim(Constants.ClaimTypeEmail, "other@example.com"),
            // ], "Test"));

            // Act
            var response = await Client.GetAsync("/api/v1/cart");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cart = await DeserializeResponse<GetCartDto>(response);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(cart);
            Assert.Equal(2, cart.Items.Count);
            Assert.Equal(20m, cart.TotalPrice);
        }

    }
}