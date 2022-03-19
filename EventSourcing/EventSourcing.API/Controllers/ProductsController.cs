using EventSourcing.API.Commands;
using EventSourcing.API.Dtos;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcing.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductCreateDto productCreateDto)
        {
            await _mediator.Send(new CreateProductCommand { ProductCreateDto = productCreateDto });
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangeName(ProductChangeNameDto productChangeNameDto)
        {
            await _mediator.Send(new ChangeProductNameCommand { ProductChangeNameDto = productChangeNameDto });
            return NoContent();
        }

        [HttpPut]
        public async Task<IActionResult> ChangePrice(ProductChangePriceDto productChangePriceDto)
        {
            await _mediator.Send(new ChangeProductPriceCommand { ProductChangePriceDto = productChangePriceDto });
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteProductCommand { Id = id });
            return NoContent();
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetProducts(int userId)
        {
            var result = await _mediator.Send(new GetProductsByUserIdQuery { UserId = userId });
            return Ok(result);
        }
    }
}
