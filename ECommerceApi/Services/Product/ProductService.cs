using System.Linq.Expressions;
using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Extensions;
using ECommerceApi.Models;
using ECommerceApi.Utils;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<Result<PagedResult<GetProductDto>>> GetAllProductsAsync(PaginationDto paginationDto)
        {
            var query = _context.Products.AsQueryable();

            var totalRecords = await query.CountAsync();
            var products = await query.Page(paginationDto).ToListAsync();
            var productsDto = products.Select(p => p.ToGetProductDto()).ToList();

            var result = PagedResultHelper.Create(productsDto, totalRecords, paginationDto);
            return Result<PagedResult<GetProductDto>>.Success(result);
        }

        public async Task<Result<PagedResult<GetProductDto>>> GetProductsFilterAsync(FilterProductDto filterProductDto)
        {
            var query = _context.Products.AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(filterProductDto.Name))
                query = query.Where(p => p.Name.Contains(filterProductDto.Name));

            var orderBySelectors = new Dictionary<ProductOrderBy, Expression<Func<Product, object>>>
            {
                [ProductOrderBy.Name] = p => p.Name!,
                [ProductOrderBy.Price] = p => p.Price!
            };

            if (orderBySelectors.TryGetValue(filterProductDto.OrderBy, out var selector))
                query = filterProductDto.AscendingOrder ? query.OrderBy(selector) : query.OrderByDescending(selector);
            else
                query = query.OrderBy(p => p.Name);

            // Apply pagination
            var totalRecords = await query.CountAsync();
            var products = await query.Page(filterProductDto).ToListAsync();
            var productsDto = products.Select(p => p.ToGetProductDto()).ToList();

            var result = PagedResultHelper.Create(productsDto, totalRecords, filterProductDto);
            return Result<PagedResult<GetProductDto>>.Success(result);
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

