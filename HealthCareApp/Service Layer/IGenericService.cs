using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace HealthCareApp.Service_Layer
{
    public interface IGenericService<A, V>
        where A : class
        where V : class
    {
        V GetById(int id);
        IEnumerable<V> GetAll();

        A Add(V entity);
        A Update(V entity);
        void Delete(V entity);
        V Find(Expression<Func<V, bool>> criteria, string[] includes = null);
        public IEnumerable<V> FindAll(Expression<Func<V, bool>> criteria, string[] includes = null);
        IEnumerable<V> FindAll(Expression<Func<V, bool>> criteria, int take, int skip);
        IEnumerable<V> FindAll(Expression<Func<V, bool>> criteria, int? take, int? skip,
            Expression<Func<V, object>> orderBy = null, string orderByDirection = OrderBy.Ascending);

        int Count();
        int Count(Expression<Func<V, bool>> criteria);
        void Save();
    }

    public static class OrderBy
    {
        public const string Ascending = "ASC";
        public const string Descending = "DESC";
    }
}
