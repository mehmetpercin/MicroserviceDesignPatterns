using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saga.Orchestration.Stock.API.Models;

namespace Saga.Orchestration.Stock.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StocksController : ControllerBase
    {
        private readonly AppDbContext _dbContext;

        public StocksController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetStocks()
        {
            var stocks = await _dbContext.Stocks.AsNoTracking().ToListAsync();
            return Ok(stocks);
        }
    }
}
