using MassTransit;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IOrderCreatedEvent : CorrelatedBy<Guid>
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
