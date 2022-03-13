namespace Saga.Orchestration.Shared.Messages
{
    public class StockRollBackMessage : IStockRollBackMessage
    {
        public List<OrderItemMessage> OrderItems { get; set; }
    }
}
