using System.Linq.Expressions;
using ECommerce.Application.Common;

namespace ECommerce.Application.Repositories;

public interface IRepository<T> where T : class
{
    Task<PagedResult<T>> GetPagedAsync(PaginationDto pagination);
    Task<T?> GetByIdAsync(int id);
    Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
    Task<List<T>> GetAllAsync();
    IQueryable<T> GetQueryable();
    Task<int> CountAsync();
    void Add(T entity);
    void Update(T entity);
    void Remove(T entity);
}
