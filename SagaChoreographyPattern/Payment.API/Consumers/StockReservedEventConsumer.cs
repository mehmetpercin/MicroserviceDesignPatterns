using MassTransit;
using Shared;

namespace Payment.API.Consumers
{
    public class StockReservedEventConsumer : IConsumer<StockReservedEvent>
    {
        private readonly ILogger<StockReservedEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndPoint;
        public StockReservedEventConsumer(ILogger<StockReservedEventConsumer> logger, IPublishEndpoint publishEndPoint)
        {
            _logger = logger;
            _publishEndPoint = publishEndPoint;
        }

        public async Task Consume(ConsumeContext<StockReservedEvent> context)
        {
            var balance = 3000m;
            if (balance > context.Message.Payment.TotalPrice)
            {
                await _publishEndPoint.Publish(new PaymentSucceededEvent
                {
                    OrderId = context.Message.OrderId,
                    UserId = context.Message.UserId
                });
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from card for user id : {context.Message.UserId}");
            }
            else
            {
                await _publishEndPoint.Publish(new PaymentFailedEvent
                {
                    OrderId = context.Message.OrderId,
                    UserId = context.Message.UserId,
                    OrderItems = context.Message.OrderItems,
                    FailMessage = "Not enough balance"
                });
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was not withdrawn from card for user id : {context.Message.UserId}");
            }
        }
    }
}
