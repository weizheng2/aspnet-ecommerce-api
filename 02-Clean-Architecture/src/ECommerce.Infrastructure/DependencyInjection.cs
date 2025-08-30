using GymManagement.Application;
using GymManagement.Infrastructure.Common;
using GymManagement.Infrastructure.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace ECommerce.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlite("Data Source=GymManagement.db");
        });
         
        services.AddScoped<ISubscriptionsRepository, SubscriptionsRepository>();
        services.AddScoped<IUnitOfWork>(ServiceProvider => ServiceProvider.GetRequiredService<ApplicationDbContext>());

        return services;
    }
}