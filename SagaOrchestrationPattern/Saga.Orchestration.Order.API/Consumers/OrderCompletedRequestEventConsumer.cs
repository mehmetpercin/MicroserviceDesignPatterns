using MassTransit;
using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestrication.Order.API.Models;

namespace Saga.Orchestration.Order.API.Consumers
{
    public class OrderCompletedRequestEventConsumer : IConsumer<IOrderCompletedRequestEvent>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrderCompletedRequestEventConsumer> _logger;

        public OrderCompletedRequestEventConsumer(AppDbContext appDbContext, ILogger<OrderCompletedRequestEventConsumer> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderCompletedRequestEvent> context)
        {
            var order = await _appDbContext.Orders.FindAsync(context.Message.OrderId);
            if (order != null)
            {
                order.Status = OrderStatus.Success;
                await _appDbContext.SaveChangesAsync();

                _logger.LogInformation($"Order (Id = {order.Id}) status changed to {order.Status}");
            }
            else
            {
                _logger.LogError($"Order (Id = {context.Message.OrderId}) not found");
            }
        }
    }
}
