// ClientApp/Services/ProductService.cs
// Copilot consolidated fetch logic into a reusable service with simple in-memory caching.
using System.Net.Http.Json;
using System.Text.Json;

namespace ClientApp.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private Product[]? _cache;
        private DateTime _cacheTimestamp;
        private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(2);

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Product[]?> GetProductsAsync(CancellationToken ct = default)
        {
            if (_cache is not null && DateTime.UtcNow - _cacheTimestamp < _cacheDuration)
            {
                return _cache;
            }

            try
            {
                // Copilot replaced hard-coded routes when the API changed from /api/products to /api/productlist.
                using var response = await _httpClient.GetAsync("/api/productlist", ct);
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync(ct);

                // Copilot recommended case-insensitive deserialization for resilience to property name variations.
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                _cache = JsonSerializer.Deserialize<Product[]>(json, options) ?? Array.Empty<Product>();
                _cacheTimestamp = DateTime.UtcNow;

                return _cache;
            }
            catch (HttpRequestException ex)
            {
                Console.Error.WriteLine($"Request error: {ex.Message}");
                return Array.Empty<Product>();
            }
            catch (JsonException ex)
            {
                Console.Error.WriteLine($"JSON error: {ex.Message}");
                return Array.Empty<Product>();
            }
            catch (TaskCanceledException)
            {
                Console.Error.WriteLine("Request canceled or timed out.");
                return Array.Empty<Product>();
            }
        }
    }

    // Copilot aligned the client models with server JSON, including nested Category.
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Price { get; set; }
        public int Stock { get; set; }
        public Category Category { get; set; } = new();
    }

    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
