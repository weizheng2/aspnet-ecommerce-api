using ECommerce.Application.Repositories;
using ECommerce.Infrastructure.Persistence;
using ECommerce.Infrastructure.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")
        ));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IUnitOfWork>(ServiceProvider => ServiceProvider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}