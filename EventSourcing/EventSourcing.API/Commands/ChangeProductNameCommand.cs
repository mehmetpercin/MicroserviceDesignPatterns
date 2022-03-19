using EventSourcing.API.Dtos;
using MediatR;

namespace EventSourcing.API.Commands
{
    public class ChangeProductNameCommand : IRequest
    {
        public ProductChangeNameDto ProductChangeNameDto { get; set; }
    }
}
