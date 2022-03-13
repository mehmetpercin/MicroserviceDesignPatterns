namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IOrderCompletedRequestEvent
    {
        public int OrderId { get; set; }
    }
}
