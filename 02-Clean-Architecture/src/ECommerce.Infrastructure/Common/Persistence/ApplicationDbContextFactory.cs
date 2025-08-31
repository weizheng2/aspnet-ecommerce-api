using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Persistence;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        
        // Point to Web/Api project's configuration
        var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ECommerce.Api"); 
        
        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json")
            .AddJsonFile("appsettings.Development.json", optional: true)
            .AddUserSecrets<ApplicationDbContextFactory>()
            .Build();

        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}