using Microsoft.AspNetCore.Mvc;

namespace Retry.ServiceB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        [HttpGet("{id}")]
        public IActionResult GetProducts(int id)
        {
            return Ok(new { Id = id, Name = "Kalem", Price = 20 });
        }
    }
}
