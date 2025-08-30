using System.Linq.Expressions;
using ECommerceApi.Data;
using ECommerceApi.Models;

namespace ECommerceApi.Repositories
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
        }

        private static readonly Dictionary<ProductOrderBy, Expression<Func<Product, object>>> OrderBySelectors = new()
        {
            [ProductOrderBy.Name] = p => p.Name,
            [ProductOrderBy.Price] = p => p.Price
        };

        public IQueryable<Product> GetFilteredQuery(ProductFilter productFilter)
        {
            var query = GetQueryable();

            if (!string.IsNullOrEmpty(productFilter.Name))
                query = query.Where(p => p.Name.Contains(productFilter.Name));

            if (OrderBySelectors.TryGetValue(productFilter.OrderBy, out var selector))
                query = productFilter.AscendingOrder ? query.OrderBy(selector) : query.OrderByDescending(selector);
            else
                query = query.OrderBy(p => p.Name);

            return query;
        }


    }
}

