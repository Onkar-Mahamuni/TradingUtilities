using Dapper;
using PortfolioApis.Models;
using System.Data;

namespace PortfolioApis.Repositories
{
    public class PositionsRepository : GenericRepository<Positions>, IPositionRepository
    {
        private readonly IDbConnection _db;
        private readonly string cagrQuery = Queries.CagrQuery;
        public PositionsRepository(IDbConnection db) : base(db)
        {
            _db = db;
        }

        public async Task<IEnumerable<Positions>> GetTradesByPositionType(string type)
        {
            var positions = await _db.QueryAsync<Positions>(cagrQuery);
            positions = positions.Where(pos => pos.PositionType == type).ToList();

            foreach (var position in positions)
            {
                var trades = await _db.QueryAsync<Trade>("SELECT * FROM TradeRecords WHERE Company = @Company", new { Company = position.Company });
                position.Trades = trades.OrderBy(trade => trade.Date).ToList();
            }

            return positions;
        }
    }
}
