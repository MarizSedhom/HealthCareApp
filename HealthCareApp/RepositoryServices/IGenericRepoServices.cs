using System.Linq.Expressions;

namespace HealthCareApp.RepositoryServices
{
    public interface IGenericRepoServices<A> where A : class
    {
        IEnumerable<A> GetAll();
        IEnumerable<A> GetAllNoTracking();
        A GetById(int id);
        public A GetById(string id);
        A GetByIdNoTracking(Func<A, bool> predicate);
        A Find(Expression<Func<A, bool>> criteria, string[] includes = null);
        //IEnumerable<A> FindAllForSearch(Expression<Func<A, bool>> criteria, string[] includes = null);
        //IEnumerable<A> FindAllForSearch(Expression<Func<A, bool>> criteria, int skip, int take, string[] includes = null);
        //IEnumerable<A> FindAllForSearch(Expression<Func<A, bool>> criteria, int? take, int? skip,
        //Expression<Func<A, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);
        public IEnumerable<A> FindAllForSearch(
        Expression<Func<A, bool>> criteria,
        int? skip = null,
        int? take = null,
        string[] includes = null,
        Expression<Func<A, object>> orderBy = null,
        string orderByDirection = OrderBy.Ascending);

        A Add(A entity);
        A UpdateNoTracking(A entity);
        void Delete(A entity);
        int Count();
        int Count(Expression<Func<A, bool>> criteria);
        void Save();
    }
    public static class OrderBy
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }
}
