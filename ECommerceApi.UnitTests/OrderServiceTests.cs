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
    public class OrderServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Mock<IUserService> _mockUserService;
        private readonly OrderService _orderService;
        private readonly User _testUser;
        private readonly User _otherUser;

        public OrderServiceTests()
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

            _orderService = new OrderService(_unitOfWork, _mockUserService.Object);

            // Set up test users
            _testUser = new User { Id = "user123", UserName = "test@example.com", Email = "test@example.com" };
            _otherUser = new User { Id = "other456", UserName = "other@example.com", Email = "other@example.com" };

            // Default mock setup - can be overridden in individual tests
            // Whenever someone calls GetValidatedUserAsync with ANY string parameter (or null), return a successful result containing _testUser
            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Success(_testUser));

            SeedTestData();
        }

        private void SeedTestData()
        {
            // Add users
            _context.Users.AddRange(_testUser, _otherUser);

            // Add products
            var products = new[]
            {
                new Product 
                { 
                    Id = 1,
                    Sku = "SKU001",
                    Name = "Product 1",
                    Price = 5m
                },
                new Product 
                { 
                    Id = 2,
                    Sku = "SKU002",
                    Name = "Product 2",
                    Price = 10m
                }
            };
            _context.Products.AddRange(products);

            // Add orders for test user
            var testUserOrders = new[]
            {
                new Order
                {
                    Id = 1,
                    UserId = _testUser.Id,
                    TotalAmount = 25m,
                    Currency = "eur",
                    PaymentStatus = "paid",
                    PaymentMethod = Constants.PaymentMethodStripe,
                    CreatedAt = DateTime.UtcNow.AddDays(-5),
                    Items = new List<OrderItem>
                    {
                        new OrderItem 
                        { 
                            Id = 1,
                            OrderId = 1,
                            ProductId = 1,
                            Quantity = 1,
                            UnitPrice = 5m
                        },
                        new OrderItem 
                        { 
                            Id = 2,
                            OrderId = 1,
                            ProductId = 2,
                            Quantity = 2,
                            UnitPrice = 10m
                        }
                    }
                },
                new Order
                {
                    Id = 2,
                    UserId = _testUser.Id,
                    TotalAmount = 10m,
                    Currency = "eur",
                    PaymentStatus = "paid",
                    PaymentMethod = Constants.PaymentMethodStripe,
                    CreatedAt = DateTime.UtcNow.AddDays(-2),
                    Items = new List<OrderItem>
                    {
                        new OrderItem 
                        { 
                            Id = 3,
                            OrderId = 2,
                            ProductId = 1,
                            Quantity = 2,
                            UnitPrice = 5m
                        }
                    }
                }
            };
            _context.Orders.AddRange(testUserOrders);

            // Add order for other user
            var otherUserOrder = new Order
            {
                Id = 3,
                UserId = _otherUser.Id,
                TotalAmount = 10m,
                Currency = "eur",
                PaymentStatus = "paid",
                PaymentMethod = Constants.PaymentMethodStripe,
                CreatedAt = DateTime.UtcNow.AddDays(-1),
                Items = new List<OrderItem>
                {
                    new OrderItem 
                    { 
                        Id = 4,
                        OrderId = 3,
                        ProductId = 2,
                        Quantity = 1,
                        UnitPrice = 10m
                    }
                }
            };
            _context.Orders.Add(otherUserOrder);
            
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetOrdersByUserAsync_ValidUser_ReturnsPagedOrders()
        {
            // Arrange
            var paginationDto = new PaginationDto { Page = 1, RecordsPerPage = 10 };

            // Act
            var result = await _orderService.GetOrdersByUserAsync(paginationDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.TotalRecords);
            Assert.Equal(2, result.Data.Data.Count);
        }

        [Fact]
        public async Task GetOrdersByUserAsync_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var paginationDto = new PaginationDto { Page = 1, RecordsPerPage = 10 };
            
            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Failure(ResultErrorType.NotFound, "User not found"));

            // Act
            var result = await _orderService.GetOrdersByUserAsync(paginationDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("User not found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetOrdersByUserAsync_UserWithNoOrders_ReturnsEmptyResult()
        {
            // Arrange
            var userWithNoOrders = new User { Id = "noOrders", UserName = "noOrders", Email = "noOrders@example.com" };
            _context.Users.Add(userWithNoOrders);
            await _context.SaveChangesAsync();

            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Success(userWithNoOrders));

            var paginationDto = new PaginationDto { Page = 1, RecordsPerPage = 10 };

            // Act
            var result = await _orderService.GetOrdersByUserAsync(paginationDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(0, result.Data.TotalRecords);
            Assert.Empty(result.Data.Data);
        }

        [Fact]
        public async Task GetOrderByUserAsync_ExistingOrder_ReturnsOrder()
        {
            // Arrange - Order belongs to Test user
            var orderId = 1;

            // Act
            var result = await _orderService.GetOrderByUserAsync(orderId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(orderId, result.Data.Id);
            Assert.NotEmpty(result.Data.Items);
            Assert.Equal(2, result.Data.Items.Count);
        }

        [Fact]
        public async Task GetOrderByUserAsync_OrderBelongsToOtherUser_ReturnsNotFound()
        {
            // Arrange - Order belongs to Other user
            var orderId = 3;

            // Act
            var result = await _orderService.GetOrderByUserAsync(orderId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("Order not found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetOrderByUserAsync_NonExistingOrder_ReturnsNotFound()
        {
            // Arrange - Non-existing order
            var orderId = 999;

            // Act
            var result = await _orderService.GetOrderByUserAsync(orderId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
            Assert.Equal("Order not found", result.ErrorMessage);
        }

        [Fact]
        public async Task GetOrderByUserAsync_InvalidUser_ReturnsNotFound()
        {
            // Arrange
            var orderId = 1;      
            _mockUserService.Setup(x => x.GetValidatedUserAsync(It.IsAny<string>())).ReturnsAsync(Result<User>.Failure(ResultErrorType.NotFound, "User not found"));

            // Act
            var result = await _orderService.GetOrderByUserAsync(orderId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task GetOrderByUserAsync_OrderWithMultipleItems_ReturnsCorrectPrices()
        {
            // Arrange - Order with 2 items
            var orderId = 1;

            // Act
            var result = await _orderService.GetOrderByUserAsync(orderId);

            // Assert
            Assert.True(result.IsSuccess);
            
            var order = result.Data;
            Assert.Equal(2, order.Items.Count);
            
            // Check individual item totals
            var item1 = order.Items.First(i => i.ProductId == 1);
            Assert.Equal(5m, item1.UnitPrice);
            Assert.Equal(1, item1.Quantity);
            Assert.Equal(5m, item1.TotalPrice);
            
            var item2 = order.Items.First(i => i.ProductId == 2);
            Assert.Equal(10m, item2.UnitPrice);
            Assert.Equal(2, item2.Quantity);
            Assert.Equal(20m, item2.TotalPrice);
            
            // Check order total (should include both items)
            Assert.Equal(25m, order.TotalAmount);
        }

        public void Dispose()
        {
            _context.Dispose();
            _unitOfWork.Dispose();
        }
    }
}