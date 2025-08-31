using ECommerce.Api.Extensions;
using ECommerce.Api.Extensions.Swagger;
using ECommerce.Application;
using ECommerce.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddHttpContextAccessor(); // Represents everything about the current HTTP Request
builder.Services.AddDataProtection();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddApplication()
                .AddInfrastructure(builder.Configuration);
 
// builder.Services.AddAllowedHostsCors(builder.Configuration);
builder.Services.AddCustomRateLimiting();
// builder.Services.AddJwtAuthentication(builder.Configuration);
// builder.Services.AddAuthorizationBasedOnPolicy();
builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();

var app = builder.Build();

// Start Middlewares
app.UseCustomSwagger();
app.UseStaticFiles();
//app.UseCors();
app.UseRateLimiter();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();


public partial class Program { }