namespace Shared
{
    public class PaymentFailedEvent
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
        public string FailMessage { get; set; }
        public List<OrderItemMessage> OrderItems { get; set; } = new List<OrderItemMessage>();
    }
}
