using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Repositories;
using ECommerceApi.Services;
using ECommerceApi.Utils;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace ECommerceApi.UnitTests
{
    public class CartServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Mock<IUserService> _mockUserService;
        private readonly CartService _cartService;
        private readonly User _testUser;
        private readonly User _otherUser;
        private Product _product1;
        private Product _product2;

        public CartServiceTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);

            // Create repository instances
            var productRepository = new ProductRepository(_context);
            var cartRepository = new CartRepository(_context);
            var orderRepository = new OrderRepository(_context);

            _unitOfWork = new UnitOfWork(_context, productRepository, cartRepository, orderRepository);
            _mockUserService = new Mock<IUserService>();

            _cartService = new CartService(_unitOfWork, _mockUserService.Object);

            // Set up test data
            _testUser = new User { Id = "user123", UserName = "test@example.com", Email = "test@example.com" };
            _otherUser = new User { Id = "other456", UserName = "other@example.com", Email = "other@example.com" };

            // Default mock setup - can be overridden in individual tests
            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Success(_testUser));

            SeedTestData();
        }

        private void SeedTestData()
        {                 
            // Add users
            _context.Users.AddRange(_testUser, _otherUser);

            // Add products
            _product1 = new Product 
            { 
                Id = 1,
                Sku = "SKU001",
                Name = "Product 1",
                Price = 5m
            };
            
            _product2 = new Product 
            { 
                Id = 2,
                Sku = "SKU002",
                Name = "Product 2",
                Price = 10m
            };
            _context.Products.AddRange(_product1, _product2);

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

            _context.Carts.AddRange(testUserCart, otherUserCart);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetCartAsync_ExistingCartWithItems_ReturnsCartWithItems()
        {
            // Act
            var result = await _cartService.GetCartAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.Items.Count);
        }

        [Fact]
        public async Task GetCartAsync_NoExistingCart_ReturnsEmptyCart()
        {
            // Arrange - Set up user with no cart
            var userWithNoCart = new User { Id = "noCart", UserName = "noCart", Email = "noCart@example.com" };
            _context.Users.Add(userWithNoCart);
            await _context.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Success(userWithNoCart));

            // Act
            var result = await _cartService.GetCartAsync();

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data.Items);
            Assert.Equal(0, result.Data.TotalPrice);
        }

        [Fact]
        public async Task GetCartAsync_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Failure(ResultErrorType.NotFound, "User not found"));

            // Act
            var result = await _cartService.GetCartAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("User not found", result.ErrorMessage);
        }

        [Fact]
        public async Task AddItemAsync_NewProduct_AddsItemToCart()
        {
            // Arrange
            var newProduct = new Product 
            { 
                Id = 3,
                Sku = "SKU003",
                Name = "Product 3",
                Price = 15m
            };
            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            var addItemDto = new AddCartItemDto
            {
                ProductId = 3,
                Quantity = 3
            };

            // Act
            var result = await _cartService.AddItemAsync(addItemDto);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify in database
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstAsync(c => c.UserId == _testUser.Id);
            
            Assert.Contains(cart.Items, i => i.ProductId == 3 && i.Quantity == 3);
        }

        [Fact]
        public async Task AddItemAsync_ExistingProduct_UpdatesQuantity()
        {
            // Arrange - Product 1 already exists with quantity 2
            var addItemDto = new AddCartItemDto
            {
                ProductId = 1,
                Quantity = 3
            };

            // Act
            var result = await _cartService.AddItemAsync(addItemDto);

            // Assert
            Assert.True(result.IsSuccess);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstAsync(c => c.UserId == _testUser.Id);
                
            var item = cart.Items.First(i => i.ProductId == 1);
            Assert.Equal(5, item.Quantity);
        }

        [Fact]
        public async Task AddItemAsync_NonExistentProduct_ReturnsNotFound()
        {
            // Arrange
            var addItemDto = new AddCartItemDto
            {
                ProductId = 999,
                Quantity = 1
            };

            // Act
            var result = await _cartService.AddItemAsync(addItemDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("Product not found", result.ErrorMessage);
        }

        [Fact]
        public async Task AddItemAsync_UserWithoutCart_CreatesCartAndAddsItem()
        {
            // Arrange - User with no existing cart
            var userWithNoCart = new User { Id = "noCart", UserName = "noCart", Email = "noCart@example.com" };
            _context.Users.Add(userWithNoCart);
            await _context.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Success(userWithNoCart));

            var addItemDto = new AddCartItemDto
            {
                ProductId = 1,
                Quantity = 1
            };

            // Act
            var result = await _cartService.AddItemAsync(addItemDto);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify cart was created
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userWithNoCart.Id);
                
            Assert.NotNull(cart);
            Assert.Single(cart.Items);
            Assert.Equal(1, cart.Items.First().ProductId);
            Assert.Equal(1, cart.Items.First().Quantity);
        }

        [Fact]
        public async Task UpdateItemAsync_ExistingItem_UpdatesQuantity()
        {
            // Arrange - Cart item 1 has quantity 2
            var updateItemDto = new UpdateCartItemDto
            {
                Quantity = 5
            };

            // Act
            var result = await _cartService.UpdateItemAsync(1, updateItemDto);

            // Assert
            Assert.True(result.IsSuccess);

            var cartItem = await _context.CartItems.FindAsync(1);
            Assert.Equal(6, cartItem.Quantity);
        }

        [Fact]
        public async Task UpdateItemAsync_ZeroQuantity_RemovesItem()
        {
            // Arrange
            var updateItemDto = new UpdateCartItemDto
            {
                Quantity = 0
            };

            // Act
            var result = await _cartService.UpdateItemAsync(1, updateItemDto);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify item was removed
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstAsync(c => c.UserId == _testUser.Id);
                
            Assert.DoesNotContain(cart.Items, i => i.Id == 1);
        }

        [Fact]
        public async Task UpdateItemAsync_NegativeQuantity_RemovesItem()
        {
            // Arrange
            var updateItemDto = new UpdateCartItemDto
            {
                Quantity = -1
            };

            // Act
            var result = await _cartService.UpdateItemAsync(1, updateItemDto);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify item was removed
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstAsync(c => c.UserId == _testUser.Id);
                
            Assert.DoesNotContain(cart.Items, i => i.Id == 1);
        }

        [Fact]
        public async Task UpdateItemAsync_NonExistentItem_ReturnsNotFound()
        {
            // Arrange
            var updateItemDto = new UpdateCartItemDto
            {
                Quantity = 5
            };

            // Act
            var result = await _cartService.UpdateItemAsync(999, updateItemDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("Cart item not found", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateItemAsync_UserWithoutCart_ReturnsNotFound()
        {
            // Arrange - User with no cart
            var userWithNoCart = new User { Id = "nocart2", UserName = "nocart2", Email = "nocart2@example.com" };
            _context.Users.Add(userWithNoCart);
            await _context.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Success(userWithNoCart));

            var updateItemDto = new UpdateCartItemDto
            {
                Quantity = 5
            };

            // Act
            var result = await _cartService.UpdateItemAsync(1, updateItemDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("Cart not found", result.ErrorMessage);
        }

        [Fact]
        public async Task ClearCartAsync_ExistingCart_ClearsAllItems()
        {
            // Act
            var result = await _cartService.ClearCartAsync();

            // Assert
            Assert.True(result.IsSuccess);

            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstAsync(c => c.UserId == _testUser.Id);
                
            Assert.Empty(cart.Items);
        }

        [Fact]
        public async Task ClearCartAsync_UserWithoutCart_ReturnsNotFound()
        {
            // Arrange - User with no cart
            var userWithNoCart = new User { Id = "nocart3", UserName = "nocart3", Email = "nocart3@example.com" };
            _context.Users.Add(userWithNoCart);
            await _context.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>()))
                .ReturnsAsync(Result<User>.Success(userWithNoCart));

            // Act
            var result = await _cartService.ClearCartAsync();

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("Cart not found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetCartTotalAmountAsync_CartWithItems_ReturnsCorrectTotal()
        {
            // Act
            var result = await _cartService.GetCartTotalAmountAsync();

            // Assert
            Assert.True(result.IsSuccess);
            
            var expectedTotal = (_product1.Price * 2) + (_product2.Price * 1);
            Assert.Equal(expectedTotal, result.Data);
        }

        public void Dispose()
        {
            _context.Dispose();
            _unitOfWork.Dispose();
        }
    }
}