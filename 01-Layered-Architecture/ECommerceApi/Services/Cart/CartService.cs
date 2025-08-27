using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Repositories;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public class CartService : ICartService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserService _userService;
        public CartService(IUnitOfWork unitOfWork, IUserService userService)
        {
            _unitOfWork = unitOfWork;
            _userService = userService;
        }
        
        private async Task<Cart> GetOrCreateCartAsync(string userId)
        {
            var cart = await _unitOfWork.Carts.GetCartWithItems(userId);
            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                _unitOfWork.Carts.Add(cart);
            }

            return cart;
        }

        public async Task<Result<GetCartDto>> GetCartAsync()
        {
            var userResult = await _userService.GetValidatedUserAsync();
            if (!userResult.IsSuccess)
                return Result<GetCartDto>.Failure(ResultErrorType.NotFound, userResult.ErrorMessage);
            var user = userResult.Data;

            var cart = await _unitOfWork.Carts.GetCartWithItemsAndProducts(user.Id);
            if (cart is null)
            {
                var emptyCart = new GetCartDto { Items = [] };
                return Result<GetCartDto>.Success(emptyCart);
            }

            return Result<GetCartDto>.Success(cart.ToGetCartDto());
        }

        public async Task<Result> AddItemAsync(AddCartItemDto addCartItemDto)
        {
            var userResult = await _userService.GetValidatedUserAsync();
            if (!userResult.IsSuccess)
                return Result.Failure(ResultErrorType.NotFound, userResult.ErrorMessage);
            var user = userResult.Data;

            var product = await _unitOfWork.Products.GetByIdAsync(addCartItemDto.ProductId);
            if (product is null)
                return Result.Failure(ResultErrorType.NotFound, "Product not found");

            var cart = await GetOrCreateCartAsync(user.Id);

            // Find existing item
            var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == addCartItemDto.ProductId);
            if (existingItem != null)
            {
                existingItem.Quantity += addCartItemDto.Quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    CartId = cart.Id,
                    ProductId = addCartItemDto.ProductId,
                    Quantity = addCartItemDto.Quantity
                });
            }

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateItemAsync(int cartItemId, UpdateCartItemDto updateCartItemDto)
        {
            var userResult = await _userService.GetValidatedUserAsync();
            if (!userResult.IsSuccess)
                return Result.Failure(ResultErrorType.NotFound, userResult.ErrorMessage);
            var user = userResult.Data;

            // Find Cart
            var cart = await _unitOfWork.Carts.GetCartWithItems(user.Id);
            if (cart is null)
                return Result.Failure(ResultErrorType.NotFound, "Cart not found");

            // Find existing item
            var cartItem = cart.Items.FirstOrDefault(item => item.Id == cartItemId);
            if (cartItem is null)
                return Result.Failure(ResultErrorType.NotFound, "Cart item not found");

            if (updateCartItemDto.Quantity <= 0)
                cart.Items.Remove(cartItem);
            else
                cartItem.Quantity = updateCartItemDto.Quantity;

            await _unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> ClearCartAsync(string? userId = null, bool saveChanges = true)
        {
            var userResult = await _userService.GetValidatedUserAsync(userId);
            if (!userResult.IsSuccess)
                return Result.Failure(ResultErrorType.NotFound, userResult.ErrorMessage);
            var user = userResult.Data;

            // Find Cart
            var cart = await _unitOfWork.Carts.GetCartWithItems(user.Id);
            if (cart is null)
                return Result.Failure(ResultErrorType.NotFound, "Cart not found");

            cart.Items.Clear();

            if(saveChanges)
                await _unitOfWork.SaveChangesAsync();
                
            return Result.Success();
        }

        public async Task<Result<decimal>> GetCartTotalAmountAsync()
        {
            var cartResult = await GetCartAsync();
            if (!cartResult.IsSuccess)
                return Result<decimal>.Failure(ResultErrorType.NotFound, "Cart not found");

            var cart = cartResult.Data;
            return Result<decimal>.Success(cart.TotalPrice);
        }
    }
    

}

