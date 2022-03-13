using Saga.Orchestration.Shared.Interfaces;

namespace Saga.Orchestration.Shared.Events
{
    public class PaymentCompletedEvent : IPaymentCompletedEvent
    {
        public PaymentCompletedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public Guid CorrelationId { get; }
    }
}
