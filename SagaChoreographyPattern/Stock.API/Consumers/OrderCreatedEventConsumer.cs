using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<OrderCreatedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly ISendEndpointProvider _sendEndpointProvider;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext dbContext, ILogger<OrderCreatedEventConsumer> logger, ISendEndpointProvider sendEndpointProvider, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _sendEndpointProvider = sendEndpointProvider;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var stockResult = new List<Tuple<int, bool>>();
            var productStocks = await _dbContext.Stocks.Where(x => context.Message.OrderItems.Select(p => p.ProductId).Contains(x.ProductId)).ToListAsync();

            context.Message.OrderItems.ForEach(item =>
            {
                stockResult.Add(Tuple.Create(item.ProductId, productStocks.Any(x => x.ProductId == item.ProductId && x.Quantity >= item.Quantity)));
            });

            if (stockResult.All(x => x.Item2.Equals(true)))
            {
                productStocks.ForEach(x =>
                {
                    x.Quantity -= context.Message.OrderItems.FirstOrDefault(o => o.ProductId == x.ProductId).Quantity;
                });

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation($"Stock was reserved for {context.Message.UserId}");
                var sendEndPoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:{RabbitMqSettingsConstants.StockReservedEventQueueName}"));
                var reservedEvent = new StockReservedEvent
                {
                    Payment = context.Message.Payment,
                    OrderId = context.Message.OrderId,
                    OrderItems = context.Message.OrderItems,
                    UserId = context.Message.UserId
                };

                await sendEndPoint.Send(reservedEvent);
            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent
                {
                    OrderId = context.Message.OrderId,
                    FailMessage = $"Stock not found for this Products :  {string.Join(",", stockResult.Where(x => !x.Item2).Select(x => x.Item1).ToList())}"
                });

                _logger.LogInformation($"Stock not found for this Products {string.Join(",", stockResult.Where(x => !x.Item2).Select(x => x.Item1).ToList())}");
            }
        }
    }
}
