using ECommerceApi.Data;
using ECommerceApi.DTOs;
using ECommerceApi.Extensions;
using ECommerceApi.Utils;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        public OrderService(ApplicationDbContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }

        public async Task<Result<PagedResult<GetOrderDto>>> GetOrdersByUserAsync(PaginationDto paginationDto)
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result<PagedResult<GetOrderDto>>.Failure(ResultErrorType.NotFound, "User not found");

            var query = _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == user.Id)
                .AsQueryable();

            var totalRecords = await query.CountAsync();
            var orders = await query.Page(paginationDto).ToListAsync();
            var orderDtos = orders.Select(o => o.ToGetOrderDto()).ToList();

            var result = PagedResultHelper.Create(orderDtos, totalRecords, paginationDto);
            return Result<PagedResult<GetOrderDto>>.Success(result);
        }

        public async Task<Result<GetOrderDto>> GetOrderByUserAsync(int orderId)
        {
            var user = await _userService.GetUser();
            if (user is null)
                return Result<GetOrderDto>.Failure(ResultErrorType.NotFound, "User not found");

            var order = await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.Id == orderId && o.UserId == user.Id)
                .FirstOrDefaultAsync();

            if (order is null)
                return Result<GetOrderDto>.Failure(ResultErrorType.NotFound, "Order not found");
      
            return Result<GetOrderDto>.Success(order.ToGetOrderDto());
        }

    }
    

}

