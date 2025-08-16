using ECommerceApi.Data;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        public OrderRepository(ApplicationDbContext context) : base(context)
        {
        }


      
    }
}