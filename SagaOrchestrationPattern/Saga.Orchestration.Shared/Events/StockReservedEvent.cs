using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Events
{
    public class StockReservedEvent : IStockReservedEvent
    {
        public StockReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }

        public List<OrderItemMessage> OrderItems { get; set; }

        public Guid CorrelationId { get; }
    }
}
