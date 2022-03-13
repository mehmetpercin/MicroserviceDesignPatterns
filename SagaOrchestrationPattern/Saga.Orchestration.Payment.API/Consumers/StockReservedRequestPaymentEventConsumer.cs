using MassTransit;
using Saga.Orchestration.Shared.Events;
using Saga.Orchestration.Shared.Interfaces;

namespace Saga.Orchestration.Payment.API.Consumers
{
    public class StockReservedRequestPaymentEventConsumer : IConsumer<IStockReservedRequestPaymentEvent>
    {
        private readonly ILogger<StockReservedRequestPaymentEventConsumer> _logger;
        private readonly IPublishEndpoint _publishEndPoint;
        public StockReservedRequestPaymentEventConsumer(ILogger<StockReservedRequestPaymentEventConsumer> logger, IPublishEndpoint publishEndPoint)
        {
            _logger = logger;
            _publishEndPoint = publishEndPoint;
        }

        public async Task Consume(ConsumeContext<IStockReservedRequestPaymentEvent> context)
        {
            var balance = 3000m;
            if (balance > context.Message.Payment.TotalPrice)
            {
                await _publishEndPoint.Publish(new PaymentCompletedEvent(context.Message.CorrelationId));
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was withdrawn from card for user id : {context.Message.BuyerId}");
            }
            else
            {
                await _publishEndPoint.Publish(new PaymentFailedEvent(context.Message.CorrelationId)
                {
                    OrderItems = context.Message.OrderItems,
                    FailMessage = "Not enough balance"
                });
                _logger.LogInformation($"{context.Message.Payment.TotalPrice} TL was not withdrawn from card for user id : {context.Message.BuyerId}");
            }
        }
    }
}
