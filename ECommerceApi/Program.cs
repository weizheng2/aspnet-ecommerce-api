using ECommerceApi.Data;
using ECommerceApi.Extensions;
using ECommerceApi.Models;
using ECommerceApi.Repositories;
using ECommerceApi.Services;
using ECommerceApi.Swagger;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

// Start Services
builder.Services.AddHttpContextAccessor(); // Represents everything about the current HTTP Request
builder.Services.AddDataProtection();
builder.Services.AddControllers().AddNewtonsoftJson();

builder.Services.AddCustomRateLimiting();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorizationBasedOnPolicy();

// builder.Services.AddAllowedHostsCors(builder.Configuration);

builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();
StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Users Database
builder.Services.AddIdentityCore<User>().AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>();

// Services 
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductService, ECommerceApi.Services.ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IPaymentService, StripePaymentService>();
builder.Services.AddScoped<IOrderService, OrderService>();

// Repositories
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    if (dbContext.Database.IsRelational())
    {
        dbContext.Database.Migrate();
    }
}

// Start Middlewares
app.UseCustomSwagger();
app.UseStaticFiles();
app.UseCors();
app.UseRateLimiter();

//app.UseExceptionLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();
app.Run();