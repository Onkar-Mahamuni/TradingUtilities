namespace PortfolioApis.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAll();
        Task<T> GetById<TId>(string idColumnName, TId id);
        Task<IEnumerable<T>> GetBySpecification(QuerySpecification<T> spec);
        void Insert(T entity);
        void Update(T entity);
        void Delete(T entity);
    }
}