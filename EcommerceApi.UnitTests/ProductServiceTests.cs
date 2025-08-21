using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Utils;
using ECommerceApi.Repositories;
using ECommerceApi.Services;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.UnitTests
{
    public class ProductServiceTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly UnitOfWork _unitOfWork;
        private readonly ProductService _productService;

        public ProductServiceTests()
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
            _productService = new ProductService(_unitOfWork);
            
            SeedTestData();
        }

        private void SeedTestData()
        {
            var products = new[]
            {
                new Product 
                { 
                    Id = 1,
                    Sku = "SKU001",
                    Name = "Test Product 1",
                    Description = "Description for product 1",
                    Price = 10.99m,
                    ImageUrl = "https://example.com/image1.jpg"
                },
                new Product 
                { 
                    Id = 2,
                    Sku = "SKU002",
                    Name = "Test Product 2",
                    Description = "Description for product 2",
                    Price = 25.50m
                },
                new Product 
                { 
                    Id = 3,
                    Sku = "SKU003",
                    Name = "Another Product",
                    Description = "Description for product 3",
                    Price = 5.99m
                }
            };

            _context.Products.AddRange(products);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetAllProductsAsync_ReturnsPagedResult()
        {
            // Arrange
            var paginationDto = new PaginationDto { Page = 1, RecordsPerPage = 10 };

            // Act
            var result = await _productService.GetAllProductsAsync(paginationDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Data.TotalRecords);
            Assert.Equal(3, result.Data.Data.Count);
            Assert.Contains(result.Data.Data, p => p.Name == "Test Product 1");
            Assert.Contains(result.Data.Data, p => p.Name == "Test Product 2");
            Assert.Contains(result.Data.Data, p => p.Name == "Another Product");
        }

        [Fact]
        public async Task GetProductsFilterAsync_WithNameFilter_ReturnsFilteredResults()
        {
            // Arrange
            var filterDto = new FilterProductDto 
            { 
                Name = "Test",
                AscendingOrder = true
            };

            // Act
            var result = await _productService.GetProductsFilterAsync(filterDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(2, result.Data.TotalRecords);
            Assert.Equal(2, result.Data.Data.Count);
            Assert.All(result.Data.Data, p => Assert.Contains("Test", p.Name));
        }

        [Fact]
        public async Task GetProductsFilterAsync_OrderByPrice_ReturnsOrderedResults()
        {
            // Arrange
            var filterDto = new FilterProductDto 
            { 
                OrderBy = ProductOrderBy.Price,
                AscendingOrder = true
            };

            // Act
            var result = await _productService.GetProductsFilterAsync(filterDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(3, result.Data.TotalRecords);
            
            var products = result.Data.Data.ToList();
            Assert.True(products[0].Price <= products[1].Price);
            Assert.True(products[1].Price <= products[2].Price);
        }

        [Fact]
        public async Task GetProductAsync_ExistingProduct_ReturnsProduct()
        {
            // Arrange
            var productId = 1;

            // Act
            var result = await _productService.GetProductAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Test Product 1", result.Data.Name);
            Assert.Equal("SKU001", result.Data.Sku);
        }

        [Fact]
        public async Task GetProductAsync_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            var productId = 999;

            // Act
            var result = await _productService.GetProductAsync(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task CreateProductAsync_ValidProduct_CreatesProduct()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Sku = "SKU004",
                Name = "New Product",
                Description = "New product description",
                Price = 15.99m
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("New Product", result.Data.Name);
            Assert.Equal("SKU004", result.Data.Sku);

            // Verify it actually in the database
            var productInDb = await _context.Products.FirstOrDefaultAsync(p => p.Sku == "SKU004");
            Assert.NotNull(productInDb);
        }

        [Fact]
        public async Task CreateProductAsync_DuplicateSku_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Sku = "SKU001",
                Name = "Duplicate SKU Product",
                Description = "This should fail",
                Price = 20.00m
            };

            // Act
            var result = await _productService.CreateProductAsync(createDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.BadRequest, result.ErrorType);
            Assert.Contains("SKU already exists", result.ErrorMessage);
        }

        [Fact]
        public async Task UpdateProductAsync_ExistingProduct_UpdatesProduct()
        {
            // Arrange
            var productId = 1;
            var updateDto = new UpdateProductDto
            {
                Name = "Updated Product Name",
                Price = 12.99m
            };

            // Act
            var result = await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify the update in the database
            var updatedProduct = await _context.Products.FindAsync(productId);
            Assert.Equal("Updated Product Name", updatedProduct.Name);
            Assert.Equal(12.99m, updatedProduct.Price);
            Assert.Equal("Description for product 1", updatedProduct.Description);
        }

        [Fact]
        public async Task UpdateProductAsync_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            var productId = 999;
            var updateDto = new UpdateProductDto
            {
                Name = "This should fail"
            };

            // Act
            var result = await _productService.UpdateProductAsync(productId, updateDto);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        }

        [Fact]
        public async Task DeleteProductAsync_ExistingProduct_DeletesProduct()
        {
            // Arrange
            var productId = 1;

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify the product is deleted from the database
            var deletedProduct = await _context.Products.FindAsync(productId);
            Assert.Null(deletedProduct);
        }

        [Fact]
        public async Task DeleteProductAsync_NonExistingProduct_ReturnsNotFound()
        {
            // Arrange
            var productId = 999;

            // Act
            var result = await _productService.DeleteProductAsync(productId);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal(ResultErrorType.NotFound, result.ErrorType);
        }

        public void Dispose()
        {
            _context.Dispose();
            _unitOfWork.Dispose();
        }
    }
}