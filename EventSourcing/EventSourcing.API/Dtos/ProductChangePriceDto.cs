namespace EventSourcing.API.Dtos
{
    public class ProductChangePriceDto
    {
        public Guid Id { get; set; }
        public decimal NewPrice { get; set; }
    }
}
