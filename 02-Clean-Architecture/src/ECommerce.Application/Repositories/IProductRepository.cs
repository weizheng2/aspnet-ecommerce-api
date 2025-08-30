
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Repositories;

public interface IProductRepository : IRepository<Product>
{
    //IQueryable<Product> GetFilteredQuery(ProductFilter productFilter);
}
