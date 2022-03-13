using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Saga.Orchestration.Shared;
using Saga.Orchestration.Shared.Events;
using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;
using Saga.Orchestrication.Order.API.Dtos;
using Saga.Orchestrication.Order.API.Models;

namespace Saga.Orchestrication.Order.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public OrdersController(AppDbContext context, ISendEndpointProvider sendEndpointProvider)
        {
            _context = context;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderCreateDto orderCreateDto)
        {
            var newOrder = new Models.Order
            {
                UserId = orderCreateDto.BuyerId,
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

            var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettingsConstants.OrderSaga}"));
            var orderCreatedRequestEvent = new OrderCreatedRequestEvent
            {
                OrderId = newOrder.Id,
                BuyerId = orderCreateDto.BuyerId,
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
            await sendEndPoint.Send<IOrderCreatedRequestEvent>(orderCreatedRequestEvent);

            return Ok();
        }
    }
}
