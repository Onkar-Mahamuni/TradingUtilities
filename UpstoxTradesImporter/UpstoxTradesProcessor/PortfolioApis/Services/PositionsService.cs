using Dapper;
using PortfolioApis.Models;
using PortfolioApis.Repositories;
using System.Data;

namespace PortfolioApis.Services
{
    public class PositionsService : IPositionsService
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly string cagrQuery = Queries.CagrQuery;

        public PositionsService(ITradeRepository tradeRepository)
        {
            _tradeRepository = tradeRepository;
        }


    }

}
