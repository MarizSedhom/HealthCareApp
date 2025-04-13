using System.Linq.Expressions;

namespace HealthCareApp.RepositoryServices
{
    public interface IGenericRepoServices<T> where T : class
    {
        T GetById(int id);
        T GetById(string id);

        IEnumerable<T> GetAll();
        T Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int take, int skip);
        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

        T Add(T entity);
        T Update(T entity);
        void SoftDelete(T entity);
        void HardDelete(T entity);
        int Count();
        int Count(Expression<Func<T, bool>> criteria);
    }
    public static class OrderBy
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }
}
