using Dapper;
using System.Data;

namespace PortfolioApis.Repositories
{
    public class QuerySpecification<T>
    {
        public Dictionary<string, List<string>> ColumnFilters { get; } = new Dictionary<string, List<string>>();
        public List<string> Orders { get; } = new List<string>();
        public bool Distinct { get; set; } = false;
        public bool UsePagination { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public List<string> SearchColumns { get; set; } = new List<string>();
    }

    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly IDbConnection _db;
        private readonly string _tableName;

        public GenericRepository(IDbConnection db)
        {
            _db = db;
            _tableName = $"{typeof(T).Name}s";
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            string query = GenerateSelectQuery();
            return await _db.QueryAsync<T>(query);
        }

        public async Task<T> GetById<TId>(string idColumnName, TId id)
        {
            string query = $"SELECT * FROM {_tableName} WHERE {idColumnName} = @Id";
            return await _db.QueryFirstOrDefaultAsync<T>(query, new { Id = id })
                    ?? throw new Exception("Entry not found");
        }

        public async Task<IEnumerable<T>> GetBySpecification(QuerySpecification<T> spec)
        {
            var parameters = new DynamicParameters();
            var whereClauses = new List<string>();
            var orderClauses = new List<string>();

            string columns = string.Join(", ", typeof(T).GetProperties().Select(p => p.Name));

            if (!string.IsNullOrEmpty(spec.SearchTerm) && spec.SearchColumns.Any())
            {
                var searchClauses = spec.SearchColumns.Select(column => $"{column} LIKE '%{spec.SearchTerm}%'");
                whereClauses.Add("(" + string.Join(" OR ", searchClauses) + ")");
            }

            // Build WHERE clause
            foreach (var filter in spec.ColumnFilters)
            {
                var values = filter.Value.Select(v => $"'{v}'");
                whereClauses.Add($"{filter.Key} IN ({string.Join(", ", values)})");
            }

            // Build ORDER BY clause
            foreach (var order in spec.Orders)
            {
                orderClauses.Add(order);
            }

            string distinct = spec.Distinct ? "DISTINCT" : string.Empty;
            string whereClause = whereClauses.Any() ? "WHERE " + string.Join(" AND ", whereClauses) : string.Empty;
            string orderClause = orderClauses.Any() ? "ORDER BY " + string.Join(", ", orderClauses) : string.Empty;

            // Add OFFSET and FETCH NEXT for pagination
            string paginationClause = spec.UsePagination ? $"OFFSET {(spec.PageNumber - 1) * spec.PageSize} ROWS FETCH NEXT {spec.PageSize} ROWS ONLY"
                : string.Empty;

            string query = $"SELECT {distinct} {columns} FROM {_tableName} {whereClause} {orderClause} {paginationClause}";

            return await _db.QueryAsync<T>(query, parameters);
        }

        public async void Insert(T entity)
        {
            var insertQuery = GenerateInsertQuery();
            await _db.ExecuteAsync(insertQuery, entity);
        }

        public async void Update(T entity)
        {
            var updateQuery = GenerateUpdateQuery();
            await _db.ExecuteAsync(updateQuery, entity);
        }

        public async void Delete(T entity)
        {
            string query = $"DELETE FROM {_tableName} WHERE Id = @Id";
            await _db.ExecuteAsync(query, entity);
        }

        private string GenerateSelectQuery()
        {
            var type = typeof(T);
            var properties = type.GetProperties();
            var columnNames = properties.Select(p => p.Name);

            string columns = string.Join(", ", columnNames);

            return $"SELECT {columns} FROM {_tableName}";
        }

        private string GenerateInsertQuery()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.Name != "Id");

            var columnNames = properties.Select(p => p.Name);
            var paramNames = properties.Select(p => "@" + p.Name);

            string columns = string.Join(", ", columnNames);
            string values = string.Join(", ", paramNames);

            return $"INSERT INTO {_tableName} ({columns}) VALUES ({values})";
        }

        private string GenerateUpdateQuery()
        {
            var type = typeof(T);
            var properties = type.GetProperties().Where(p => p.Name != "Id");

            var setClause = properties.Select(p => p.Name + " = @" + p.Name);

            return $"UPDATE {_tableName} SET {string.Join(", ", setClause)} WHERE Id = @Id";
        }
    }
}
