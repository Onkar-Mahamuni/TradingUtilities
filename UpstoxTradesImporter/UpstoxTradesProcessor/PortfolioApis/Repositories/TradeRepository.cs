using Dapper;
using PortfolioApis.Models;
using System.Data;
using System.Data.Common;

namespace PortfolioApis.Repositories
{
    public class TradeRepository : GenericRepository<Trade>, ITradeRepository
    {
        private readonly IDbConnection _db;
        private readonly string cagrQuery = Queries.CagrQuery;

        public TradeRepository(IDbConnection db) : base(db)
        {
            
            _db = db;
        }

        public async Task<List<Positions>> GetTradesByPositionType(string type)
        {
            var positions = await _db.QueryAsync<Positions>(cagrQuery);
            positions = positions.Where(pos => pos.PositionType == type).ToList();

            List<string> companies = positions.Select(pos => pos.Company).ToList();

            var spec = new QuerySpecification<Trade>
            {
                ColumnFilters =
                {
                    { nameof(Trade.Company), companies }
                },
            };

            var trades = await GetBySpecification(spec);

            foreach (var position in positions)
            {
                position.Trades = trades.Where(trade => trade.Company == position.Company).OrderBy(trade => trade.Date).ToList();
            }

            return positions.ToList();
        }
    }

}
