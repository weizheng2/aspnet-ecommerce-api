using ECommerceApi.Data;

namespace ECommerceApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public IProductRepository Products { get; }
        public ICartRepository Carts { get; }
        public IOrderRepository Orders { get; }

        public UnitOfWork(ApplicationDbContext context, IProductRepository products, ICartRepository carts, IOrderRepository orders)
        {
            _context = context;
            Products = products;
            Carts = carts;
            Orders = orders;
        }

        public async Task<int> SaveChangesAsync() => await _context.SaveChangesAsync();

        public void Dispose() => _context.Dispose();
    }
}