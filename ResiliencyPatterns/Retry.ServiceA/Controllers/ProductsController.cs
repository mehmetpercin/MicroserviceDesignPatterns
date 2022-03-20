using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Retry.ServiceA.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProduct(int id)
        {
            return Ok(await _productService.GetProductById(id));
        }
    }
}
