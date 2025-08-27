using ECommerceApi.DTOs;
using ECommerceApi.Extensions;
using ECommerceApi.Repositories;
using ECommerceApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PagedResult<GetProductDto>>> GetAllProductsAsync(PaginationDto paginationDto)
        {
            var query = _unitOfWork.Products.GetQueryable();

            var totalRecords = await query.CountAsync();
            var productsDto = await query.Page(paginationDto)
                            .Select(p => p.ToGetProductDto())
                            .ToListAsync();

            var result = PagedResultHelper.Create(productsDto, totalRecords, paginationDto);
            return Result<PagedResult<GetProductDto>>.Success(result);
        }

        public async Task<Result<PagedResult<GetProductDto>>> GetProductsFilterAsync(FilterProductDto filterProductDto)
        {
            var filter = new ProductFilter
            {
                Name = filterProductDto.Name,
                OrderBy = filterProductDto.OrderBy,
                AscendingOrder = filterProductDto.AscendingOrder
            };

            var query = _unitOfWork.Products.GetFilteredQuery(filter);

            // Apply pagination
            var totalRecords = await query.CountAsync();
            var products = await query.Page(filterProductDto).ToListAsync();
            var productsDto = products.Select(p => p.ToGetProductDto()).ToList();

            var result = PagedResultHelper.Create(productsDto, totalRecords, filterProductDto);
            return Result<PagedResult<GetProductDto>>.Success(result);
        }

        public async Task<Result<GetProductDto>> GetProductAsync(int id)
        {
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return Result<GetProductDto>.Failure(ResultErrorType.NotFound);
     
            return Result<GetProductDto>.Success(product.ToGetProductDto());
        }

        public async Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto)
        {
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Sku == productDto.Sku);
            if (product != null)
                return Result<GetProductDto>.Failure(ResultErrorType.BadRequest, "Product with the same SKU already exists.");

            product = productDto.ToProduct();
            _unitOfWork.Products.Add(product);
            await _unitOfWork.SaveChangesAsync();

            return Result<GetProductDto>.Success(product.ToGetProductDto());
        }

        public async Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto)
        {
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return Result.Failure(ResultErrorType.NotFound);

            // Update only fields that are provided
            if (productDto.Name != null) product.Name = productDto.Name;
            if (productDto.Description != null) product.Description = productDto.Description;
            if (productDto.Price.HasValue) product.Price = productDto.Price.Value;
            if (productDto.ImageUrl != null) product.ImageUrl = productDto.ImageUrl;

            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteProductAsync(int id)
        {
            var product = await _unitOfWork.Products.FirstOrDefaultAsync(p => p.Id == id);
            if (product == null)
                return Result.Failure(ResultErrorType.NotFound);

            _unitOfWork.Products.Remove(product);
            await _unitOfWork.SaveChangesAsync();

            return Result.Success();
        }

    }

}

