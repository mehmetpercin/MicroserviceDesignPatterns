using Saga.Orchestration.Shared.Interfaces;

namespace Saga.Orchestration.Shared.Events
{
    public class OrderCompletedRequestEvent : IOrderCompletedRequestEvent
    {
        public int OrderId { get; set; }
    }
}
