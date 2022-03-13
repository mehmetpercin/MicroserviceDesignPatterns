using MassTransit;
using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestrication.Order.API.Models;

namespace Saga.Orchestration.Order.API.Consumers
{
    public class OrderFailedRequestEventConsumer : IConsumer<IOrderFailedRequestEvent>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<OrderFailedRequestEventConsumer> _logger;

        public OrderFailedRequestEventConsumer(AppDbContext appDbContext, ILogger<OrderFailedRequestEventConsumer> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<IOrderFailedRequestEvent> context)
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
