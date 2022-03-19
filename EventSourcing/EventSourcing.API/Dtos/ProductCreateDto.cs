namespace EventSourcing.API.Dtos
{
    public class ProductCreateDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int UserId { get; set; }
    }
}
