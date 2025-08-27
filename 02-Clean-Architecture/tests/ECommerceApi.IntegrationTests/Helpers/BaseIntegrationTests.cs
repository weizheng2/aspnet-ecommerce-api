using Microsoft.Extensions.DependencyInjection;
using ECommerceApi.Data;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ECommerceApi.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
    }

    public abstract class BaseIntegrationTest : IClassFixture<CustomWebApplicationFactory>, IAsyncLifetime
    {
        protected readonly WebApplicationFactory<Program> Factory;
        protected readonly HttpClient Client;
        protected readonly string DatabaseName = $"testdb_{Guid.NewGuid()}";

        protected static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected BaseIntegrationTest(CustomWebApplicationFactory factory)
        {
            // Create a factory with the specific database name for this test instance
            Factory = factory.WithWebHostBuilder(builder =>
            {
                builder.UseEnvironment("Testing");

                builder.ConfigureServices(services =>
                {
                    // Remove existing DbContextOptions configuration if registered
                    var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<ApplicationDbContext>));
                    if (descriptor != null)
                        services.Remove(descriptor);

                    // Also remove the DbContext itself if registered
                    var contextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ApplicationDbContext));
                    if (contextDescriptor != null)
                        services.Remove(contextDescriptor);

                    // Add mock authentication
                    services.AddSingleton<ITestUserProvider, TestUserProvider>();
                    services.AddAuthentication("Test").AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                    // Use the consistent database name for this test instance
                    services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase(DatabaseName));
                });
            });

            Client = Factory.CreateClient();
        }

        public virtual async Task InitializeAsync()
        {
            await SeedTestData();
        }

        public virtual Task DisposeAsync() => Task.CompletedTask;

        protected virtual async Task SeedTestData()
        {
            using var scope = Factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            // Clear existing data
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();

            await SeedSpecificTestData(context);
        }

        protected virtual Task SeedSpecificTestData(ApplicationDbContext context)
        {
            return Task.CompletedTask;
        }

        protected async Task<T?> DeserializeResponse<T>(HttpResponseMessage response)
        {
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, JsonOptions);
        }

        protected ApplicationDbContext GetFreshContext()
        {
            var scope = Factory.Services.CreateScope();
            return scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        }

        protected void SetCustomUserAuth(string email, bool authorized = true, bool isAdmin = false)
        {
            using var scope = Factory.Services.CreateScope();
            var provider = scope.ServiceProvider.GetRequiredService<ITestUserProvider>();
            
            var claims = new List<Claim>
            {
                new(Constants.ClaimTypeEmail, email)
            };

            if (isAdmin)
            {
                claims.Add(new Claim(Constants.PolicyIsAdmin, "true"));
            }

            provider.CurrentUser = new ClaimsPrincipal(new ClaimsIdentity(claims, authorized ? "Test" : ""));
        }
    }
}