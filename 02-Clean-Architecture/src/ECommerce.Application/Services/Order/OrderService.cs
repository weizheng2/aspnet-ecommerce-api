// using ECommerceApi.DTOs;
// using ECommerceApi.Extensions;
// using ECommerceApi.Repositories;
// using ECommerceApi.Utils;
// using Microsoft.EntityFrameworkCore;

// namespace ECommerceApi.Services
// {
//     public class OrderService : IOrderService
//     {
//         private readonly IUnitOfWork _unitOfWork;
//         private readonly IUserService _userService;
//         public OrderService(IUnitOfWork unitOfWork, IUserService userService)
//         {
//             _unitOfWork = unitOfWork;
//             _userService = userService;
//         }

//         public async Task<Result<PagedResult<GetOrderDto>>> GetOrdersByUserAsync(PaginationDto paginationDto)
//         {
//             var userResult = await _userService.GetValidatedUserAsync();
//             if (!userResult.IsSuccess)
//                 return Result<PagedResult<GetOrderDto>>.Failure(ResultErrorType.NotFound, userResult.ErrorMessage);
//             var user = userResult.Data;

//             var query = _unitOfWork.Orders.QueryUserOrdersWithDetails(user.Id);

//             var totalRecords = await query.CountAsync();
//             var orderDtos = await query.Page(paginationDto)
//                             .Select(o => o.ToGetOrderDto())
//                             .ToListAsync();

//             var result = PagedResultHelper.Create(orderDtos, totalRecords, paginationDto);
//             return Result<PagedResult<GetOrderDto>>.Success(result);
//         }

//         public async Task<Result<GetOrderDto>> GetOrderByUserAsync(int orderId)
//         {
//             var userResult = await _userService.GetValidatedUserAsync();
//             if (!userResult.IsSuccess)
//                 return Result<GetOrderDto>.Failure(ResultErrorType.NotFound, userResult.ErrorMessage);
//             var user = userResult.Data;

//             var order = await _unitOfWork.Orders.GetUserOrderWithDetailsAsync(user.Id, orderId);
//             if (order is null)
//                 return Result<GetOrderDto>.Failure(ResultErrorType.NotFound, "Order not found");
      
//             return Result<GetOrderDto>.Success(order.ToGetOrderDto());
//         }

//     }
    

// }

