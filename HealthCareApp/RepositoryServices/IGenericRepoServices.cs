using System.Linq.Expressions;

namespace HealthCareApp.RepositoryServices
{
    public interface IGenericRepoServices<T> where T : class
    {
        T GetById(int id);
        T GetById(string id);

        IEnumerable<T> GetAll();

        T Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        public TResult FindWithSelect<TResult>(Expression<Func<T, bool>> criteria, Expression<Func<T, TResult>> selector, params Expression<Func<T, object>>[] includes);

        IEnumerable<TResult> FindAllWithSelect<TResult>(Expression<Func<T, bool>> criteria, Expression<Func<T, TResult>> selector, params Expression<Func<T, object>>[] includes);
        public IEnumerable<TResult> FindAllWithSelectIgnoreFilter<TResult>(Expression<Func<T, bool>> criteria, Expression<Func<T, TResult>> selector, params Expression<Func<T, object>>[] includes);

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes);

        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int take, int skip);

        IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? take, int? skip,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

        T Add(T entity);
        void AddRange(IEnumerable<T> entity);

        T Update(T entity);
        void SoftDelete(T entity);
        void SoftDeleteRange(IEnumerable<T> entities);

        void HardDelete(T entity);
        void HardDeleteRange(IEnumerable<T> entities);

        int Count();
        int Count(Expression<Func<T, bool>> criteria);
        void SaveChanges();
        public IEnumerable<T> GetAllNoTracking();
        public T GetByIdNoTracking(Func<T, bool> predicate);
        public IEnumerable<T> FindAllForSearch(
        Expression<Func<T, bool>> criteria,
        int? skip = null,
        int? take = null,
        string[] includes = null,
        Expression<Func<T, object>> orderBy = null,
        string orderByDirection = OrderBy.Ascending);

        public IEnumerable<TResult> FindAllWithSelectForSearch<TResult>(
            Expression<Func<T, bool>> criteria
            , Expression<Func<T, TResult>> selector,
            int? skip = null,
            int? take = null,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
         );
        public T UpdateNoTracking(T entity);
        public void Save();
    }
    public static class OrderBy
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }
}