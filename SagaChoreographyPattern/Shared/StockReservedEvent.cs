namespace Shared
{
    public class StockReservedEvent
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public PaymentMessage Payment { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
    }
}
