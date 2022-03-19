namespace EventSourcing.API.Dtos
{
    public class ProductChangeNameDto
    {
        public Guid Id { get; set; }
        public string NewName { get; set; }
    }
}
