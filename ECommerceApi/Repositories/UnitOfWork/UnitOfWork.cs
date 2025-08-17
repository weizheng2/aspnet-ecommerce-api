using ECommerceApi.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace ECommerceApi.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private IDbContextTransaction? _currentTransaction;

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

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }
    }
}