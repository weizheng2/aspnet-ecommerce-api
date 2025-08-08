using ECommerceApi.DTOs;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface IProductService
    {
        Task<Result<List<GetProductDto>>> GetAllProductsAsync();
        Task<Result<List<GetProductDto>>> GetProductsFilterAsync();
        Task<Result<GetProductDto>> GetProductAsync(int id);
        Task<Result<GetProductDto>> CreateProductAsync(CreateProductDto productDto);
        Task<Result> UpdateProductAsync(int id, UpdateProductDto productDto);
        Task<Result> DeleteProductAsync(int id);
    }
}

