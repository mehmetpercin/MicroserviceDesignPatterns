using MassTransit;
using Order.API.Models;
using Shared;

namespace Order.API.Consumers
{
    public class PaymentSucceededEventConsumer : IConsumer<PaymentSucceededEvent>
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<PaymentSucceededEventConsumer> _logger;

        public PaymentSucceededEventConsumer(AppDbContext appDbContext, ILogger<PaymentSucceededEventConsumer> logger)
        {
            _appDbContext = appDbContext;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<PaymentSucceededEvent> context)
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
