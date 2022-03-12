using MassTransit;
using Microsoft.EntityFrameworkCore;
using Shared;
using Stock.API.Models;

namespace Stock.API.Consumers
{
    public class PaymentFailedEventConsumer : IConsumer<PaymentFailedEvent>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<PaymentFailedEventConsumer> _logger;

        public PaymentFailedEventConsumer(AppDbContext dbContext, ILogger<PaymentFailedEventConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
        {
            var productStocks = await _dbContext.Stocks.Where(x => context.Message.OrderItems.Select(p => p.ProductId).Contains(x.ProductId)).ToListAsync();
            foreach (var productStock in productStocks)
            {
                productStock.Quantity += context.Message.OrderItems.First(x => x.ProductId == productStock.ProductId).Quantity;
            }

            await _dbContext.SaveChangesAsync();
            _logger.LogInformation($"Stocks back reserved for this Products {string.Join(", ", productStocks.Select(x => x.ProductId).ToList())}");
        }
    }
}
