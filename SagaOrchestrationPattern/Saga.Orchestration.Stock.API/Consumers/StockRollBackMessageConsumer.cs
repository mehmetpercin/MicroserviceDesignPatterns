using MassTransit;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Shared.Messages;
using Saga.Orchestration.Stock.API.Models;

namespace Saga.Orchestration.Stock.API.Consumers
{
    public class StockRollBackMessageConsumer : IConsumer<IStockRollBackMessage>
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<StockRollBackMessageConsumer> _logger;

        public StockRollBackMessageConsumer(AppDbContext dbContext, ILogger<StockRollBackMessageConsumer> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IStockRollBackMessage> context)
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
