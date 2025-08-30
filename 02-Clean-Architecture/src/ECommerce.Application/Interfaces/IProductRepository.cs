using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public interface IProductRepository : IRepository<Product>
    {
        IQueryable<Product> GetFilteredQuery(ProductFilter productFilter);
    }
}