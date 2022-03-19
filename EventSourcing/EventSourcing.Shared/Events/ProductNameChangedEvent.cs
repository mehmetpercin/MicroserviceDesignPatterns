namespace EventSourcing.Shared.Events
{
    public class ProductNameChangedEvent : IEvent
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }
}
