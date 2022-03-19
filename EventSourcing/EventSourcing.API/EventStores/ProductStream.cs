using EventSourcing.API.Dtos;
using EventSourcing.Shared.Events;
using EventStore.ClientAPI;

namespace EventSourcing.API.EventStores
{
    public class ProductStream : AbstractStream
    {
        public static string StreamName => "ProductStream";
        public static string GroupName => "Replay";
        public ProductStream(IEventStoreConnection eventStoreConnection) : base(StreamName, eventStoreConnection)
        {
        }

        public void Created(ProductCreateDto productCreateDto)
        {
            Events.AddLast(new ProductCreatedEvent
            {
                Id = Guid.NewGuid(),
                Name = productCreateDto.Name,
                Price = productCreateDto.Price,
                Stock = productCreateDto.Stock,
                UserId = productCreateDto.UserId
            });
        }

        public void NameChanged(ProductChangeNameDto productChangeNameDto)
        {
            Events.AddLast(new ProductNameChangedEvent
            {
                Id = productChangeNameDto.Id,
                NewName = productChangeNameDto.NewName
            });
        }

        public void PriceChanged(ProductChangePriceDto productChangePriceDto)
        {
            Events.AddLast(new ProductPriceChangedEvent
            {
                Id = productChangePriceDto.Id,
                NewPrice = productChangePriceDto.NewPrice
            });
        }

        public void Deleted(Guid id)
        {
            Events.AddLast(new ProductDeletedEvent
            {
                Id = id
            });
        }
    }
}
