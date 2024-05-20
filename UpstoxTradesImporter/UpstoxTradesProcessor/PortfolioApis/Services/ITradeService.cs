using PortfolioApis.Models;

namespace PortfolioApis.Services
{
    public interface ITradeService
    {
        Task<List<Positions>> GetTradesByPositionType(string positionType);
    }
}