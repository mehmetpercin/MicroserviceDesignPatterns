using EventSourcing.API.Dtos;
using EventSourcing.API.Models;
using EventSourcing.API.Queries;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace EventSourcing.API.Handlers
{
    public class GetProductsByUserIdQueryHandler : IRequestHandler<GetProductsByUserIdQuery, List<ProductDto>>
    {
        private readonly AppDbContext _context;

        public GetProductsByUserIdQueryHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<ProductDto>> Handle(GetProductsByUserIdQuery request, CancellationToken cancellationToken)
        {
            var products = await _context.Products.Where(x => x.UserId == request.UserId).Select(x => new ProductDto
            {
                Id = x.Id,
                Name = x.Name,
                UserId = x.UserId,
                Price = x.Price,
                Stock = x.Stock
            }).ToListAsync(cancellationToken);
            return products;
        }
    }
}
