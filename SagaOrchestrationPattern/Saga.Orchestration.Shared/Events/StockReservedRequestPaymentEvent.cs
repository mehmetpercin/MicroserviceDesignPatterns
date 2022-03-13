using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Events
{
    public class StockReservedRequestPaymentEvent : IStockReservedRequestPaymentEvent
    {
        public StockReservedRequestPaymentEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
        public string BuyerId { get; set; }
    }
}
