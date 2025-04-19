//using System.Linq.Expressions;
//using HealthCareApp.RepositoryServices;
//using Mapster;


//namespace HealthCareApp.Service_Layer
//{
//    public class GenericService<A, V> : IGenericService<A, V>
//    where A : class
//    where V : class
//    {
//        protected readonly IGenericRepoServices<A> genericRepoServices;

//        public GenericService(IGenericRepoServices<A> genericRepoServices)
//        {
//            this.genericRepoServices = genericRepoServices;
//        }

//        public int Count()
//        {
//            return genericRepoServices.Count();
//        }

//        public int Count(Expression<Func<V, bool>> criteria)
//        {
//            var entityCriteria = ExpressionConverter.Convert<A, V>(criteria);
//            return genericRepoServices.Count(entityCriteria);
//        }

//        public void Delete(V entity)
//        {
//            var entityToDelete = entity.Adapt<A>();
//            genericRepoServices.Delete(entityToDelete);
//        }
        
//        public void DeleteWithComposite(V entity, params object[] keyValues)
//        {
//            var entityToDelete = entity.Adapt<A>();
//            genericRepoServices.DeleteWithComposite(entityToDelete, keyValues);
//        }

//        public V Find(Expression<Func<V, bool>> criteria, string[] includes = null)
//        {
//            var entityCriteria = ExpressionConverter.Convert<A, V>(criteria);
//            var result = genericRepoServices.Find(entityCriteria, includes);
//            return result?.Adapt<V>();
//        }

//        public IEnumerable<V> FindAllForSearch(Expression<Func<V, bool>> criteria, string[] includes = null)
//        {
//            var entityCriteria = ExpressionConverter.Convert<A, V>(criteria);
//            return genericRepoServices.FindAllForSearch(entityCriteria, includes).ToList().Adapt<IEnumerable<V>>();
//        }

//        public IEnumerable<V> FindAllForSearch(Expression<Func<V, bool>> criteria, int take, int skip)
//        {
//            var entityCriteria = ExpressionConverter.Convert<A, V>(criteria);
//            return genericRepoServices.FindAllForSearch(entityCriteria, take, skip).Adapt<IEnumerable<V>>();
//        }

//        public IEnumerable<V> FindAllForSearch(Expression<Func<V, bool>> criteria, int? take, int? skip, Expression<Func<V, object>> orderBy = null, string orderByDirection = "ASC")
//        {
//            var entityCriteria = ExpressionConverter.Convert<A, V>(criteria);
//            var entityOrderBy = orderBy?.Adapt<Expression<Func<A, object>>>();
//            return genericRepoServices.FindAllForSearch(entityCriteria, take, skip, entityOrderBy, orderByDirection).Adapt<IEnumerable<V>>();
//        }

//        public IEnumerable<V> GetAll()
//        {
//            return genericRepoServices.GetAll().Adapt<IEnumerable<V>>();
//        }

//        public IEnumerable<V> GetAllWithNoTracking()
//        {
//            return genericRepoServices.GetAllNoTracking().Adapt<IEnumerable<V>>();
//        }

//        public V GetById(int id)
//        {
//            return genericRepoServices.GetById(id).Adapt<V>();
//        }

//        public V GetByIdNoTracking(Expression<Func<V, bool>> criteria)
//        {
//            var entityCriteria = ExpressionConverter.Convert<A, V>(criteria);
//            return genericRepoServices.Find(entityCriteria).Adapt<V>();
//        }

//        public void Save()
//        {
//            genericRepoServices.Save();
//        }

//        public void Clear()
//        {
//            genericRepoServices.Clear();
//        }

//        public A UpdateNoTracking(V entity)
//        {
//            var entityToUpdate = entity.Adapt<A>();
//            var updatedEntity = genericRepoServices.UpdateNoTracking(entityToUpdate);
//            return updatedEntity;
//        }

//        public A Add(V entity)
//        {
//            var entityToAdd = entity.Adapt<A>();
//            var addedEntity = genericRepoServices.Add(entityToAdd);
//            return addedEntity;
//        }
//    }
//}

