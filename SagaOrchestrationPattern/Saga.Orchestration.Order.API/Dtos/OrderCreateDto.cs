namespace Saga.Orchestrication.Order.API.Dtos
{
    public class OrderCreateDto
    {
        public string BuyerId { get; set; }
        public List<OrderItemDto> OrderItems { get; set; }
        public PaymentDto Payment { get; set; }
        public AddressDto Address { get; set; }
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }

    public class PaymentDto
    {
        public string CardName { get; set; }
        public string CardNumber { get; set; }
        public string Expiration { get; set; }
        public string CVV { get; set; }
    }

    public class AddressDto
    {
        public string Line { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
    }
}
