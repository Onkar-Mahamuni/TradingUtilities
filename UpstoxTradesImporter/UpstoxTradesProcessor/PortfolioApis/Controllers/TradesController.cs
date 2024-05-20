using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PortfolioApis.Services;

namespace PortfolioApis.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ITradeService _tradeService;

        public TradesController(ITradeService tradeService)
        {
            _tradeService = tradeService;
        }

        [HttpGet("open-positions")]
        public async Task<IActionResult> GetOpenPositionsDetails()
        {
            var openPositions = await _tradeService.GetTradesByPositionType("Closed Positions");
            return Ok(openPositions);
        }

        [HttpGet("closed-positions")]
        public async Task<IActionResult> GetClosedPositionsDetails()
        {
            var closedPositions = await _tradeService.GetTradesByPositionType("Open Positions");
            return Ok(closedPositions);
        }
    }

}
