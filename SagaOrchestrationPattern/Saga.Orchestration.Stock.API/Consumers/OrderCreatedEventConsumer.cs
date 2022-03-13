using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Shared.Events;
using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Stock.API.Models;

namespace Saga.Orchestration.Stock.API.Consumers
{
    public class OrderCreatedEventConsumer : IConsumer<IOrderCreatedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<OrderCreatedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndpoint;

        public OrderCreatedEventConsumer(AppDbContext dbContext, ILogger<OrderCreatedEventConsumer> logger, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _logger = logger;
            _publishEndpoint = publishEndpoint;
        }

        public async Task Consume(ConsumeContext<IOrderCreatedEvent> context)
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

                _logger.LogInformation($"Stock was reserved for CorrelationId : {context.Message.CorrelationId}");
                var reservedEvent = new StockReservedEvent(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems
                };

                await _publishEndpoint.Publish(reservedEvent);
            }
            else
            {
                await _publishEndpoint.Publish(new StockNotReservedEvent(context.Message.CorrelationId)
                {
                    FailMessage = $"Stock not found for this Products :  {string.Join(",", stockResult.Where(x => !x.Item2).Select(x => x.Item1).ToList())}"
                });

                _logger.LogInformation($"Stock not found for this Products {string.Join(",", stockResult.Where(x => !x.Item2).Select(x => x.Item1).ToList())}");
            }
        }
    }
}
