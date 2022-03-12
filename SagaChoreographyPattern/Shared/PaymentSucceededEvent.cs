namespace Shared
{
    public class PaymentSucceededEvent
    {
        public int OrderId { get; set; }
        public string UserId { get; set; }
    }
}
