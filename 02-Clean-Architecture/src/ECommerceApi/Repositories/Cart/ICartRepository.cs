using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public interface ICartRepository : IRepository<Cart>
    {
        Task<Cart?> GetCartWithItems(string userId); 
        Task<Cart?> GetCartWithItemsAndProducts(string userId); 
    }
}