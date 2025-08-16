using System.Linq.Expressions;

namespace ECommerceApi.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<List<T>> GetAllAsync();
        IQueryable<T> GetQueryable();
        Task<int> CountAsync();
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
    }
}