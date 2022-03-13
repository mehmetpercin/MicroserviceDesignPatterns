using MassTransit;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IStockNotReservedEvent : CorrelatedBy<Guid>
    {
        public string FailMessage { get; set; }
    }
}
