using ECommerceApi.DTOs;
using ECommerceApi.Utils;

namespace ECommerceApi.Services
{
    public interface IOrderService
    {
        Task<Result<PagedResult<GetOrderDto>>> GetOrdersByUserAsync (PaginationDto paginationDto);
        Task<Result<GetOrderDto>> GetOrderByUserAsync (int orderId);

    }
}
