using System.Net;
using System.Net.Http.Json;
using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Utils;

namespace ECommerceApi.IntegrationTests
{
    public class ProductsControllerIntegrationTests : BaseIntegrationTest
    {
        private Product _product1;
        private Product _product2;
        private Product _product3;
        private User _testUser;
        private string _testUserEmail = "test@example.com";

        public ProductsControllerIntegrationTests(CustomWebApplicationFactory factory) : base(factory)
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
            context.Users.Add(_testUser);

            // Add products for testing
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
                Name = "Other Product",
                Price = 15m
            };

            context.Products.AddRange(_product1, _product2, _product3);
            await context.SaveChangesAsync();
        }

        [Fact]
        public async Task GetAllProducts_ReturnsPagedResult()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/products");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await DeserializeResponse<PagedResult<GetProductDto>>(response);
            Assert.NotNull(result);
            Assert.Equal(3, result.Data.Count);
        }

        [Fact]
        public async Task GetProductsWithFilter_WithNameFilter_ReturnsMatchingProducts()
        {
            // Act
            var response = await Client.GetAsync("/api/v1/products/filter?Name=Other");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await DeserializeResponse<PagedResult<GetProductDto>>(response);
            Assert.NotNull(result);
            Assert.Contains(result.Data, p => p.Name.Contains("Other", StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public async Task GetProduct_WithExistingId_ReturnsProduct()
        {
            // Act
            var response = await Client.GetAsync($"/api/v1/products/{_product1.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var product = await DeserializeResponse<GetProductDto>(response);
            Assert.NotNull(product);
            Assert.Equal(_product1.Id, product.Id);
        }

        [Fact]
        public async Task CreateProduct_WithValidDataAsAdmin_ReturnsCreated()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, true);

            var createDto = new CreateProductDto
            {
                Sku = "SKU004",
                Name = "New Product",
                Price = 25.99m,
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/products", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            var createdProduct = await DeserializeResponse<GetProductDto>(response);
            Assert.NotNull(createdProduct);
            Assert.Equal(createDto.Name, createdProduct.Name);
            Assert.Equal(createDto.Sku, createdProduct.Sku);
        }

        [Fact]
        public async Task CreateProduct_WithDuplicateSku_ReturnsBadRequest()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, true);

            var createDto = new CreateProductDto
            {
                Sku = "SKU001", // Duplicate SKU
                Name = "Duplicate SKU Product",
                Price = 25.99m
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/products", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_AsRegularUser_ReturnsForbidden()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, false);

            var createDto = new CreateProductDto
            {
                Sku = "SKU006",
                Name = "Unauthorized Product",
                Price = 25.99m
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/products", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithoutAuthentication_ReturnsForbidden()
        {
            // Arrange
            SetCustomUserAuth("", false);

            var createDto = new CreateProductDto
            {
                Sku = "SKU006",
                Name = "Unauthenticated Product",
                Price = 25.99m
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/products", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task CreateProduct_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, true);

            var createDto = new CreateProductDto
            {
                Sku = "", // Invalid: empty SKU
                Name = "",
                Price = -10m
            };

            // Act
            var response = await Client.PostAsJsonAsync("/api/v1/products", createDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_WithValidDataAsAdmin_ReturnsNoContent()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, true);

            var updateDto = new UpdateProductDto
            {
                Name = "Updated Laptop Computer",
                Price = 1099.99m,
            };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/v1/products/{_product1.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the product was updated
            var getResponse = await Client.GetAsync($"/api/v1/products/{_product1.Id}");
            var updatedProduct = await DeserializeResponse<GetProductDto>(getResponse);
            Assert.NotNull(updatedProduct);
            Assert.Equal(updateDto.Name, updatedProduct.Name);
            Assert.Equal(updateDto.Price, updatedProduct.Price);
        }

        [Fact]
        public async Task UpdateProduct_AsRegularUser_ReturnsForbidden()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, false);

            var updateDto = new UpdateProductDto
            {
                Name = "Unauthorized Update",
                Price = 50m
            };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/v1/products/{_product1.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task UpdateProduct_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, true);

            var updateDto = new UpdateProductDto
            {
                Name = "", // Invalid: empty name
                Price = -50m // Invalid: negative price
            };

            // Act
            var response = await Client.PutAsJsonAsync($"/api/v1/products/{_product1.Id}", updateDto);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_WithValidIdAsAdmin_ReturnsNoContent()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, true);

            // Act
            var response = await Client.DeleteAsync($"/api/v1/products/{_product3.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

            // Verify the product was deleted
            var getResponse = await Client.GetAsync($"/api/v1/products/{_product3.Id}");
            Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_AsRegularUser_ReturnsForbidden()
        {
            // Arrange
            SetCustomUserAuth(_testUserEmail, true, false);

            // Act
            var response = await Client.DeleteAsync($"/api/v1/products/{_product1.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task DeleteProduct_WithoutAuthentication_ReturnsForbidden()
        {
            // Arrange
            SetCustomUserAuth("", false);

            // Act
            var response = await Client.DeleteAsync($"/api/v1/products/{_product1.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

    }
}