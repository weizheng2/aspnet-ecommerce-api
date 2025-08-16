using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public interface IOrderRepository : IRepository<Order>
    {
        Task<Order?> GetUserOrderWithDetailsAsync(string userId, int orderId);
        IQueryable<Order> QueryUserOrdersWithDetails(string userId);

    }
}