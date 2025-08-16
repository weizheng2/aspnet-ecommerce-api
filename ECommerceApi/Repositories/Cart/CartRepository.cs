using ECommerceApi.Data;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public class CartRepository : Repository<Cart>, ICartRepository
    {
        public CartRepository(ApplicationDbContext context) : base(context)
        {
        }


      
    }
}