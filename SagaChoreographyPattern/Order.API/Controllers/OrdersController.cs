using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Order.API.Dtos;
using Order.API.Models;
using Shared;

namespace Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrdersController(AppDbContext context, IPublishEndpoint publishEndpoint)
        {
            _context = context;
            _publishEndpoint = publishEndpoint;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                UserId = orderCreateDto.UserId,
                Status = OrderStatus.Suspend,
                Address = new Address
                {
                    District = orderCreateDto.Address.District,
                    Line = orderCreateDto.Address.Line,
                    Province = orderCreateDto.Address.Province
                },
                FailMessage = string.Empty,
                CreatedDate = DateTime.Now,
                OrderItems = orderCreateDto.OrderItems.Select(x => new OrderItem { Price = x.Price, ProductId = x.ProductId, Quantity = x.Quantity }).ToList(),
            };

            await _context.AddAsync(newOrder);
            await _context.SaveChangesAsync();

            var orderCreatedEvent = new OrderCreatedEvent
            {
                OrderId = newOrder.Id,
                UserId = orderCreateDto.UserId,
                OrderItems = newOrder.OrderItems.Select(x => new OrderItemMessage { ProductId = x.ProductId, Quantity = x.Quantity }).ToList(),
                Payment = new PaymentMessage
                {
                    CardName = orderCreateDto.Payment.CardName,
                    CardNumber = orderCreateDto.Payment.CardNumber,
                    CVV = orderCreateDto.Payment.CVV,
                    Expiration = orderCreateDto.Payment.Expiration,
                    TotalPrice = newOrder.OrderItems.Sum(x => x.Price * x.Quantity)
                }
            };

            await _publishEndpoint.Publish(orderCreatedEvent);

            return Ok();
        }
    }
}
