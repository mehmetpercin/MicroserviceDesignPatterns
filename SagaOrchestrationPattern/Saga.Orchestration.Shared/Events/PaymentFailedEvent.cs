using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Events
{
    public class PaymentFailedEvent : IPaymentFailedEvent
    {
        public PaymentFailedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public string FailMessage { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}
