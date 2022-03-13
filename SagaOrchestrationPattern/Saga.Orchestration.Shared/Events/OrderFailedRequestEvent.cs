using Saga.Orchestration.Shared.Interfaces;

namespace Saga.Orchestration.Shared.Events
{
    public class OrderFailedRequestEvent : IOrderFailedRequestEvent
    {
        public int OrderId { get; set; }
        public string FailMessage { get; set; }
    }
}
