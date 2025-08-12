using ECommerceApi.DTOs;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface ICartService
    {
        Task<Result<GetCartDto>> GetCartAsync();
        Task<Result> AddItemAsync(AddCartItemDto addCartItemDto);
        Task<Result> UpdateItemAsync(int cartItemId, UpdateCartItemDto updateCartItemDto);
        // Task<Result> RemoveCartItemAsync(int id);
        // Task<Result> ClearCartAsync(int id);

    }
}
