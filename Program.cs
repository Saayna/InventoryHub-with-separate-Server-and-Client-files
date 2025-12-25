using Microsoft.Extensions.Caching.Memory;
using ServerApp.Models;

var builder = WebApplication.CreateBuilder(args);

// Copilot recommended adding CORS and caching services for integration and performance.
builder.Services.AddCors();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Copilot suggested permissive CORS during development; lock down in production.
app.UseCors(policy =>
    policy.AllowAnyOrigin()
          .AllowAnyMethod()
          .AllowAnyHeader());

app.MapGet("/api/productlist", (IMemoryCache cache) =>
{
    // Copilot proposed caching the product list to reduce server load.
    if (!cache.TryGetValue("products", out Product[] products))
    {
        products = new[]
        {
            new Product
            {
                Id = 1,
                Name = "Laptop",
                Price = 1200.50,
                Stock = 25,
                Category = new Category { Id = 101, Name = "Electronics" }
            },
            new Product
            {
                Id = 2,
                Name = "Headphones",
                Price = 50.00,
                Stock = 100,
                Category = new Category { Id = 102, Name = "Accessories" }
            }
        };

        // Copilot suggested a reasonable cache duration for dev; make configurable later.
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));
        cache.Set("products", products, cacheOptions);
    }

    // Copilot recommended Results.Json for explicit control (optional; returning products also works).
    return Results.Json(products);
});

app.Run();
