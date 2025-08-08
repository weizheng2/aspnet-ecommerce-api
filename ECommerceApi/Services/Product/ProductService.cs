using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class ProductService : IProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<List<GetProductDto>>> GetAllProductsAsync()
        {
            var products = await _context.Products.ToListAsync();
            var productsDto = products.Select(p => p.ToGetProductDto()).ToList();

            return Result<List<GetProductDto>>.Success(productsDto);
        }

        public async Task<Result<List<GetProductDto>>> GetProductsFilterAsync()
        {
            // Todo - Implement filtering logic here
            var products = await _context.Products.ToListAsync();
            var productsDto = products.Select(p => p.ToGetProductDto()).ToList();

            return Result<List<GetProductDto>>.Success(productsDto);
        }

        public async Task<Result<GetProductDto>> GetProductAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return Result<GetProductDto>.Failure(ResultErrorType.NotFound);
     
            return Result<GetProductDto>.Success(product.ToGetProductDto());
        }

        public async Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Sku == productDto.Sku);
            if (product != null)
                return Result<GetProductDto>.Failure(ResultErrorType.BadRequest, "Product with the same SKU already exists.");
    
            product = productDto.ToProduct();
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            
            return Result<GetProductDto>.Success(product.ToGetProductDto());
        }

        public async Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return Result.Failure(ResultErrorType.NotFound);

            // Update only fields that are provided
            if (productDto.Name != null) product.Name = productDto.Name;
            if (productDto.Description != null) product.Description = productDto.Description;
            if (productDto.Price.HasValue) product.Price = productDto.Price.Value;
            if (productDto.ImageUrl != null) product.ImageUrl = productDto.ImageUrl;

            await _context.SaveChangesAsync();
            
            return Result.Success();
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return Result.Failure(ResultErrorType.NotFound);

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Result.Success();
        }

    }

}

