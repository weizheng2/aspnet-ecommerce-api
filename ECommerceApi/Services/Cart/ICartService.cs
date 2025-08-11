using ECommerceApi.DTOs;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface ICartService
    {
        Task<Result<GetCartDto>> GetCartAsync();
        Task<Result> AddItemAsync(AddCartItemDto addCartItemDto);
        // Task<Result> UpdateCartAsync(int id, UpdateProductDto productDto);
        // Task<Result> RemoveCartItemAsync(int id);
        // Task<Result> ClearCartAsync(int id);

    }
}
