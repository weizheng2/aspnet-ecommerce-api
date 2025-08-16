using ECommerceApi.Data;
using ECommerceApi.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerceApi.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Order?> GetUserOrderWithDetailsAsync(string userId, int orderId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);
        }

        public IQueryable<Order> QueryUserOrdersWithDetails(string userId)
        {
            return _context.Orders
                .Include(o => o.Items)
                    .ThenInclude(oi => oi.Product)
                .Where(o => o.UserId == userId)
                .AsQueryable();
        }

    }
}