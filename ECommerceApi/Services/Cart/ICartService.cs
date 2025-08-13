using ECommerceApi.DTOs;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface ICartService
    {
        Task<Result<GetCartDto>> GetCartAsync();
        Task<Result> AddItemAsync(AddCartItemDto addCartItemDto);
        Task<Result> UpdateItemAsync(int cartItemId, UpdateCartItemDto updateCartItemDto);
        Task<Result> ClearCartAsync(string userId = null);
        Task<Result<decimal>> GetCartTotalAmountAsync();

    }
}
