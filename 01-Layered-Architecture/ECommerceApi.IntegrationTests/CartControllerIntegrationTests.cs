using System.Net;
using System.Net.Http.Json;
using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;

namespace ECommerceApi.IntegrationTests
{
    public class CartControllerIntegrationTests : BaseIntegrationTest
    {
        private Product _product1;
        private Product _product2;
        private Product _product3;
        private User _testUser;
        private User _emptyCartUser;
        private User _noCartUser;
        private string _testUserEmail = "test@example.com";
        private string _emptyCartUserEmail = "emptyCart@example.com";
        private string _noCartUserEmail = "noCart@example.com";

        public CartControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        protected override async Task SeedSpecificTestData(ApplicationDbContext context)
        {
            // Add users
            _testUser = new User
            {
                Id = "test-user-123",
                UserName = _testUserEmail,
                Email = _testUserEmail,
                // We need to set these since we are using context.Users and not UserManager
                NormalizedEmail = _testUserEmail.ToUpperInvariant(),
                NormalizedUserName = _testUserEmail.ToUpperInvariant()
            };

            _emptyCartUser = new User
            {
                Id = "empty-cart-user-456",
                UserName = _emptyCartUserEmail,
                Email = _emptyCartUserEmail,
                NormalizedEmail = _emptyCartUserEmail.ToUpperInvariant(),
                NormalizedUserName = _emptyCartUserEmail.ToUpperInvariant()
            };

            _noCartUser = new User
            {
                Id = "no-cart-user-789",
                UserName = _noCartUserEmail,
                Email = _noCartUserEmail,
                NormalizedEmail = _noCartUserEmail.ToUpperInvariant(),
                NormalizedUserName = _noCartUserEmail.ToUpperInvariant()
            };
            context.Users.AddRange(_testUser, _emptyCartUser, _noCartUser);

            // Add products
            _product1 = new Product
            {
                Id = 1,
                Sku = "SKU001",
                Name = "Test Product 1",
                Price = 5m,
            };

            _product2 = new Product
            {
                Id = 2,
                Sku = "SKU002",
                Name = "Test Product 2",
                Price = 10m
            };

            _product3 = new Product
            {
                Id = 3,
                Sku = "SKU003",
                Name = "Test Product 3",
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
                UserId = _emptyCartUser.Id,
                Items = new List<CartItem>()
            };
            context.Carts.AddRange(testUserCart, otherUserCart);

            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetCart_WithExistingCart_ReturnsOkCartData()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/cart");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cart = await DeserializeResponse<GetCartDto>(response);

            Assert.NotNull(cart);
            Assert.Equal(2, cart.Items.Count);
            Assert.Equal(20m, cart.TotalPrice);
        }

        [Fact]
        public async Task AddItemToCart_WithoutAuthentication_ReturnsForbidden()
        {
            // Arrange
            SetCustomUserAuth("", false);

            var addItemDto = new AddCartItemDto
            {
                ProductId = _product1.Id,
                Quantity = 1
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/cart", addItemDto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task AddItemToCart_WithValidProduct_ReturnsOkAndAddsItem()
        {
            // Arrange
            var addItemDto = new AddCartItemDto
            {
                ProductId = _product3.Id,
                Quantity = 2
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/cart", addItemDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify item was added to cart
            var getResponse = await Client.GetAsync("/api/v1/cart");
            var cart = await DeserializeResponse<GetCartDto>(getResponse);
            Assert.NotNull(cart);
            Assert.Equal(3, cart.Items.Count);
            Assert.Contains(cart.Items, i => i.ProductId == _product3.Id && i.Quantity == 2);
        }

        [Fact]
        public async Task AddItemToCart_WithExistingProduct_IncreasesQuantity()
        {
            // Arrange
            var addItemDto = new AddCartItemDto
            {
                ProductId = _product1.Id, // This product already exists in cart with quantity 2
                Quantity = 3
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/cart", addItemDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify quantity was increased
            var getResponse = await Client.GetAsync("/api/v1/cart");
            var cart = await DeserializeResponse<GetCartDto>(getResponse);
            Assert.NotNull(cart);

            var updatedItem = cart.Items.FirstOrDefault(i => i.ProductId == _product1.Id);
            Assert.NotNull(updatedItem);
            Assert.Equal(5, updatedItem.Quantity); // 2 + 3 = 5
        }

        [Fact]
        public async Task AddItemToCart_WithNonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            var addItemDto = new AddCartItemDto
            {
                ProductId = 999, // Non-existent product
                Quantity = 1
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/cart", addItemDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddItemToCart_WithNegativeQuantity_ReturnsBadRequest()
        {
            // Arrange
            var addItemDto = new AddCartItemDto
            {
                ProductId = _product1.Id,
                Quantity = -1
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/cart", addItemDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AddItemToCart_NoExistingCart_CreatesCartAndAddsItem()
        {
            // Arrange
            SetCustomUserAuth(_noCartUserEmail);

            var addItemDto = new AddCartItemDto
            {
                ProductId = _product1.Id,
                Quantity = 1
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/cart", addItemDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify cart now has the item
            var getResponse = await Client.GetAsync("/api/v1/cart");
            var cart = await DeserializeResponse<GetCartDto>(getResponse);
            Assert.NotNull(cart);
            Assert.Single(cart.Items);
            Assert.Contains(cart.Items, i => i.ProductId == _product1.Id && i.Quantity == 1);
        }

        [Fact]
        public async Task UpdateCartItem_WithValidData_ReturnsOkAndUpdatesQuantity()
        {
            // Arrange
            var updateDto = new UpdateCartItemDto { Quantity = 5 };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/v1/cart/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify cart was updated
            var getResponse = await Client.GetAsync("/api/v1/cart");
            var cart = await DeserializeResponse<GetCartDto>(getResponse);
            Assert.NotNull(cart);

            var updatedItem = cart.Items.FirstOrDefault(i => i.ProductId == _product1.Id);
            Assert.NotNull(updatedItem);
            Assert.Equal(5, updatedItem.Quantity);
        }

        [Fact]
        public async Task UpdateCartItem_WithZeroQuantity_ReturnsOkAndRemovesItem()
        {
            // Arrange
            var updateDto = new UpdateCartItemDto { Quantity = 0 };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/v1/cart/1", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            // Verify cart was updated
            var getResponse = await Client.GetAsync("/api/v1/cart");
            var cart = await DeserializeResponse<GetCartDto>(getResponse);
            Assert.NotNull(cart);

            var updatedItem = cart.Items.FirstOrDefault(i => i.ProductId == _product1.Id);
            Assert.Null(updatedItem);
        }

        [Fact]
        public async Task ClearCart_WithExistingCart_ReturnsNoContentAndClearsCart()
        {
            // Act
            var response = await Client.DeleteAsync("/api/v1/cart/clear");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify cart is empty
            var getResponse = await Client.GetAsync("/api/v1/cart");
            var cart = await DeserializeResponse<GetCartDto>(getResponse);
            Assert.NotNull(cart);
            Assert.Empty(cart.Items);
            Assert.Equal(0m, cart.TotalPrice);
        }

        [Fact]
        public async Task ClearCart_WithEmptyCart_ReturnsNoContent()
        {
            // Arrange
            SetCustomUserAuth(_emptyCartUserEmail);

            // Act
            var response = await Client.DeleteAsync("/api/v1/cart/clear");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }

        [Fact]
        public async Task GetCart_CalculatesTotalPriceCorrectly()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/cart");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var cart = await DeserializeResponse<GetCartDto>(response);
            Assert.NotNull(cart);

            // Product1: 2 * 5 = 10, Product2: 1 * 10 = 10, Total = 20
            Assert.Equal(20m, cart.TotalPrice);
            Assert.Equal(2, cart.Items.Count);
        }
    }
}