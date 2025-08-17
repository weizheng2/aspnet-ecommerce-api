namespace ECommerceApi.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        ICartRepository Carts { get; }
        IOrderRepository Orders { get; }

        Task<int> SaveChangesAsync();
    }
}