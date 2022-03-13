namespace Saga.Orchestration.Shared.Interfaces
{
    public interface IOrderFailedRequestEvent
    {
        public int OrderId { get; set; }
        public string FailMessage { get; set; }
    }
}
