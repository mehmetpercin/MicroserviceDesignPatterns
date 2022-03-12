namespace Shared
{
    public class StockNotReservedEvent
    {
        public int OrderId { get; set; }
        public string FailMessage { get; set; }
    }
}
