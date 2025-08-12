using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Models;
using ECommerceApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class CartService : ICartService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        public CartService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        private async Task<Cart?> GetOrCreateCartAsync(string userId, bool createIfNotExists = true)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null && createIfNotExists)
            {
                cart = new Cart { UserId = userId };
                _context.Carts.Add(cart);
                await _context.SaveChangesAsync();
            }

            return cart;
        }

        public async Task<Result<GetCartDto>> GetCartAsync()
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result<GetCartDto>.Failure(ResultErrorType.NotFound);

            var cart = await _context.Carts
               .Include(c => c.Items)
                   .ThenInclude(ci => ci.Product)
               .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart is null)
            {
                var emptyCart = new GetCartDto { Items = [] };
                return Result<GetCartDto>.Success(emptyCart);
            }

            return Result<GetCartDto>.Success(cart.ToGetCartDto());
        }

        public async Task<Result> AddItemAsync(AddCartItemDto addCartItemDto)
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result.Failure(ResultErrorType.NotFound, "User not found");

            var product = await _context.Products.FindAsync(addCartItemDto.ProductId);
            if (product is null)
                return Result.Failure(ResultErrorType.NotFound, "Product not found");

            var cart = await GetOrCreateCartAsync(user.Id);

             // Find existing item
            var existingItem = cart!.Items.FirstOrDefault(item => item.ProductId == addCartItemDto.ProductId);
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

            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> UpdateItemAsync(int cartItemId, UpdateCartItemDto updateCartItemDto)
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result.Failure(ResultErrorType.NotFound, "User not found");

            // Find Cart
            var cart = await GetOrCreateCartAsync(user.Id, false);
    
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
    
            await _context.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> ClearCartAsync()
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result.Failure(ResultErrorType.NotFound, "User not found");

            // Find Cart
            var cart = await GetOrCreateCartAsync(user.Id, false);
            if (cart is null)
                return Result.Failure(ResultErrorType.NotFound, "Cart not found");

            cart.Items.Clear();
            await _context.SaveChangesAsync();
            return Result.Success();
        }
    }

}

