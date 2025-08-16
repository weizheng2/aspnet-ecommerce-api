namespace ECommerceApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }
        IUserRepository Users { get; }

        Task<int> SaveChangesAsync();
    }
}