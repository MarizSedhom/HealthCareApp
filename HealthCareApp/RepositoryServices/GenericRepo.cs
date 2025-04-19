using HealthCareApp.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace HealthCareApp.RepositoryServices
{
    public class GenericRepo<T> : IGenericRepoServices<T> where T : class
    {
        protected ApplicationDbContext _context;

        public GenericRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> GetAll()
        {
            return _context.Set<T>().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        public T GetById(string id)
        {
            return _context.Set<T>().Find(id);
        }

        public T Find(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.SingleOrDefault(criteria);
        }


        public TResult FindWithSelect<TResult>( Expression<Func<T, bool>> criteria,Expression<Func<T, TResult>> selector,params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            return query
                .Where(criteria)
                .Select(selector)
                .FirstOrDefault();
        }


        public IEnumerable<TResult> FindAllWithSelect<TResult>(Expression<Func<T, bool>> criteria, Expression<Func<T, TResult>> selector, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }
            if (criteria == null)
                return query.Select(selector).ToList();
            else
                return query.Where(criteria).Select(selector).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            return query.Where(criteria).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int skip, int take)
        {
            return _context.Set<T>().Where(criteria).Skip(skip).Take(take).ToList();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> criteria, int? skip, int? take,
            Expression<Func<T, object>> orderBy = null, string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                    query = query.OrderBy(orderBy);
                else
                    query = query.OrderByDescending(orderBy);
            }

            return query.ToList();
        }
        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();

            return entity;
        }
        public void AddRange(IEnumerable<T> entity)
        {
            _context.Set<T>().AddRange(entity);
            _context.SaveChanges();

        }

        public T Update(T entity)
        {
            _context.Update(entity);
            _context.SaveChanges();

            return entity;
        }

        public void SoftDelete(T entity)
        {
            //_context.Set<T>().Remove(entity);
            PropertyInfo property = entity.GetType().GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);
                _context.Entry(entity).State = EntityState.Modified;
            }

            _context.SaveChanges();
        }
        public void SoftDeleteRange(IEnumerable< T> entities)
        {
            //_context.Set<T>().Remove(entity);
            foreach (T entity in entities)
            {
                PropertyInfo property = entity.GetType().GetProperty("IsDeleted");
                if (property != null && property.PropertyType == typeof(bool))
                {
                    property.SetValue(entity, true);
                    _context.Entry(entity).State = EntityState.Modified;
                }
            }
            _context.SaveChanges();
        }

        public void HardDelete(T entity)
        {
            _context.Set<T>().Remove(entity);
            _context.SaveChanges();
        }
        public void HardDeleteRange(IEnumerable<T> entities)
        {
            _context.Set<T>().RemoveRange(entities);
            _context.SaveChanges();
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }
        public void SaveChanges()
        {
            _context.SaveChanges();
        }
        public IEnumerable<T> GetAllNoTracking()
        {
            return _context.Set<T>().AsNoTracking().ToList();
        }
        public T GetByIdNoTracking(Func<T, bool> predicate)
        {
            return _context.Set<T>()
                           .AsNoTracking()
                           .FirstOrDefault(predicate);
        }
        public IEnumerable<T> FindAllForSearch(
       Expression<Func<T, bool>> criteria,
       int? skip = null,
       int? take = null,
       string[] includes = null,
       Expression<Func<T, object>> orderBy = null,
       string orderByDirection = OrderBy.Ascending)
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                {
                    query = query.OrderBy(orderBy);
                }
                else if (orderByDirection == OrderBy.Descending)
                {
                    query = query.OrderByDescending(orderBy);
                }
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);

            return query.ToList();
        }
        public IEnumerable<TResult> FindAllWithSelectForSearch<TResult>(
            Expression<Func<T, bool>> criteria
            ,Expression<Func<T, TResult>>selector,
            int? skip = null,
            int? take = null,
            string[] includes = null,
            Expression<Func<T, object>> orderBy = null,
            string orderByDirection = OrderBy.Ascending
         )
        {
            IQueryable<T> query = _context.Set<T>().Where(criteria);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
            {
                if (orderByDirection == OrderBy.Ascending)
                {
                    query = query.OrderBy(orderBy);
                }
                else if (orderByDirection == OrderBy.Descending)
                {
                    query = query.OrderByDescending(orderBy);
                }
            }

            if (skip.HasValue)
                query = query.Skip(skip.Value);

            if (take.HasValue)
                query = query.Take(take.Value);
            
            return query.Select(selector).ToList();
        }
        public T UpdateNoTracking(T entity)
        {
            var dbEntity = _context.Set<T>().Find(entity.GetType().GetProperty("Id")?.GetValue(entity));

            if (dbEntity != null)
            {
                _context.Entry(dbEntity).State = EntityState.Detached;
            }

            _context.Update(entity);

            return entity;
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
////
