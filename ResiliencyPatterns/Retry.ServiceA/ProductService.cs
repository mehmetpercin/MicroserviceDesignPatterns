using Retry.ServiceA.Models;

namespace Retry.ServiceA
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ProductService> _logger;

        public ProductService(HttpClient httpClient, ILogger<ProductService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<Product> GetProductById(int id)
        {
            var product = await _httpClient.GetFromJsonAsync<Product>($"{id}");
            _logger.LogInformation($"Product -> Id : {product.Id} , Name : {product.Name}");
            return product;
        }
    }
}
