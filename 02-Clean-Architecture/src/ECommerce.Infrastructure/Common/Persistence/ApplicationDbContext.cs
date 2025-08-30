using System.Reflection;
using ECommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Persistence
{
    public class ApplicationDbContext : DbContext //IdentityDbContext<User>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // modelBuilder.Entity<OrderItem>().Property(o => o.UnitPrice).HasPrecision(18, 2);
            // modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            base.OnModelCreating(modelBuilder);
        }
        
        public async Task CommitChangesAsync()
        {
            await base.SaveChangesAsync();
        }


        public DbSet<Product> Products { get; set; }
        // public DbSet<Cart> Carts { get; set; }
        // public DbSet<CartItem> CartItems { get; set; }
        // public DbSet<Order> Orders { get; set; }
        // public DbSet<OrderItem> OrderItems { get; set; }
        // public DbSet<Error> Errors { get; set; }

    }
}