using Microsoft.AspNetCore.Mvc;
using Asp.Versioning;
using Microsoft.AspNetCore.RateLimiting;
using ECommerceApi.DTOs;
using ECommerceApi.Services;
using Microsoft.AspNetCore.Authorization;
using ECommerceApi.Utils;

namespace ECommerceApi.Controllers
{
    [ApiVersion("1.0")]
    [EnableRateLimiting(Constants.RateLimitGeneral)]
    [ControllerName("Order"), Tags("Order")]
    [ApiController, Route("api/v{version:apiVersion}/order")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedResult<GetOrderDto>>> GetOrdersByUser([FromQuery] PaginationDto paginationDto)
        {
            var result = await _orderService.GetOrdersByUserAsync(paginationDto);
            if (result.IsSuccess)
                return Ok(result.Data);

            return NotFound(result.ErrorMessage);
        }

        [HttpGet("{orderId}")]
        [Authorize]
        public async Task<ActionResult<GetOrderDto>> GetOrderByUser(int orderId)
        {
            var result = await _orderService.GetOrderByUserAsync(orderId);
            if (result.IsSuccess)
                return Ok(result.Data);

            return NotFound(result.ErrorMessage);
        }

    }
}


