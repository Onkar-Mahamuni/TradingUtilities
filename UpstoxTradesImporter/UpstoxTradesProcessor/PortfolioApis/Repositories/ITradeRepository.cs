using PortfolioApis.Models;

namespace PortfolioApis.Repositories
{
    public interface ITradeRepository : IGenericRepository<Trade>
    {
        Task<List<Positions>> GetTradesByPositionType(string type);
    }
}