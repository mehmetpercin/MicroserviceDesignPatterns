namespace Order.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserId { get; set; }
        public Address Address { get; set; }
        public OrderStatus Status { get; set; }
        public string FailMessage { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }

    public enum OrderStatus
    {
        Suspend,
        Success,
        Fail
    }
}
