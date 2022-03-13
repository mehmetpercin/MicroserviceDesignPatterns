using Saga.Orchestration.Shared.Interfaces;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Events
{
    public class StockNotReservedEvent : IStockNotReservedEvent
    {
        public StockNotReservedEvent(Guid correlationId)
        {
            CorrelationId = correlationId;
        }
        public string FailMessage { get; set; }

        public Guid CorrelationId { get; }
    }
}
