using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using ECommerceApi.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ECommerceApi.Controllers
{
    [ApiVersion("1.0")]
    [EnableRateLimiting(Constants.RateLimitGeneral)]
    [ControllerName("Cart"), Tags("Cart")]
    [ApiController, Route("api/v{version:apiVersion}/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<GetCartDto>> GetCart()
        {
            var result = await _cartService.GetCartAsync();

            if (result.IsSuccess)
                return Ok(result.Data);

            return NotFound();
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> AddItemToCart(AddCartItemDto addCartItemDto)
        {
            var result = await _cartService.AddItemAsync(addCartItemDto);
            if (result.IsSuccess)
                return Ok();

            switch (result.ErrorType)
            {
                case ResultErrorType.NotFound: return NotFound(result.ErrorMessage);
                default: return BadRequest(result.ErrorMessage);
            }
        }

        [HttpPut("{cartItemId}")]
        [Authorize]
        public async Task<ActionResult> UpdateCartItem(int cartItemId, UpdateCartItemDto updateCartItemDto)
        {
            var result = await _cartService.UpdateItemAsync(cartItemId, updateCartItemDto);
            if (result.IsSuccess)
                return Ok(new { cartItemId, quantity = updateCartItemDto.Quantity });

            return NotFound(result.ErrorMessage);
        }

        [HttpDelete("clear")]
        [Authorize]
        public async Task<ActionResult> ClearCart()
        {
            var result = await _cartService.ClearCartAsync();
            if (result.IsSuccess)
                return NoContent();

            return NotFound(result.ErrorMessage);
        }
    }
}


