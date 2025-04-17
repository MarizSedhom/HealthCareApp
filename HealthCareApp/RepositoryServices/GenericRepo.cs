using HealthCareApp.Data;
using Microsoft.EntityFrameworkCore;
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

        public IEnumerable<T> GetAllNoTracking()
        {
            return _context.Set<T>().AsNoTracking().ToList();
        }

        public T GetById(int id)
        {
            return _context.Set<T>().Find(id);
        }
        
        public T GetById(string id)
        {
            return _context.Set<T>().Find(id);
        }

        public T GetByIdNoTracking(Func<T, bool> predicate)
        {
            return _context.Set<T>()
                           .AsNoTracking()
                           .FirstOrDefault(predicate);
        }

        public T Find(Expression<Func<T, bool>> criteria, string[] includes = null)
        {
            IQueryable<T> query = _context.Set<T>();

            if (includes != null)
                foreach (var incluse in includes)
                    query = query.Include(incluse);

            return query.SingleOrDefault(criteria);
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

        public T Add(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
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

        public void Delete(T entity)
        {
            PropertyInfo property = entity.GetType().GetProperty("IsDeleted");
            if (property != null && property.PropertyType == typeof(bool))
            {
                property.SetValue(entity, true);
            }
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public int Count(Expression<Func<T, bool>> criteria)
        {
            return _context.Set<T>().Count(criteria);
        }

        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
