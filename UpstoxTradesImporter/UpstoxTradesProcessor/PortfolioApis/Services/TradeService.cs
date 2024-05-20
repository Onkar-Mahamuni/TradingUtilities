using PortfolioApis.Models;
using PortfolioApis.Repositories;

namespace PortfolioApis.Services
{
    public class TradeService : ITradeService
    {
        private readonly ITradeRepository _tradeRepository;

        public TradeService(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }

        public async Task<List<Positions>> GetTradesByPositionType(string positionType)
        {
            return await _tradeRepository.GetTradesByPositionType(positionType);
        }
    }
}
