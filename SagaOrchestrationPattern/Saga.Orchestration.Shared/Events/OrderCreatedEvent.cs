using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Events
{
    public class OrderCreatedEvent : IOrderCreatedEvent
    {
        public OrderCreatedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}
