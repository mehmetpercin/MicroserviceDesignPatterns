using MediatR;

namespace EventSourcing.API.Commands
{
    public class DeleteProductCommand : IRequest
    {
        public Guid Id { get; set; }
    }
}
