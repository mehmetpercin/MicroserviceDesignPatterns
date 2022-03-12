using MassTransit;
using Order.API.Models;
using Shared;

namespace Order.API.Consumers
{
    public class StockNotReservedEventConsumer : IConsumer<StockNotReservedEvent>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<StockNotReservedEventConsumer> _logger;

        public StockNotReservedEventConsumer(AppDbContext appDbContext, ILogger<StockNotReservedEventConsumer> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }
        public async Task Consume(ConsumeContext<StockNotReservedEvent> context)
        {
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Fail;
                order.FailMessage = context.Message.FailMessage;
                await _appDbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id = {order.Id}) status changed to {order.Status}.Fail message : {order.FailMessage}");
            }
            else
            {
                _logger.LogError($"Order (Id = {context.Message.OrderId}) not found");
            }
        }
    }
}
