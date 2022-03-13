using MassTransit;

namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IPaymentCompletedEvent : CorrelatedBy<Guid>
    {
    }
}
