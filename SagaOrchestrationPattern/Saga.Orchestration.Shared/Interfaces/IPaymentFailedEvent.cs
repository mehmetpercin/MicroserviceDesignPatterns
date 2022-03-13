using MassTransit;
using Saga.Orchestration.Shared.Messages;

namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IPaymentFailedEvent  : CorrelatedBy<Guid>
    {
        public string FailMessage { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
