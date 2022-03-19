namespace EventSourcing.Shared.Events
{
    public class ProductPriceChangedEvent : IEvent
    {
        public Guid Id { get; set; }
        public decimal NewPrice { get; set; }
    }
}
