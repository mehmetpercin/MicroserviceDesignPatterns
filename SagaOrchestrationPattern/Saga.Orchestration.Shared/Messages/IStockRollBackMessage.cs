namespace Saga.Orchestration.Shared.Messages
{
    public interface IStockRollBackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
