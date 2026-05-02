using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentCore;
using InfrastructureManagmentDataAccess.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _appDBContext;
        public BaseRepository(AppDbContext appDBContext) {
        
          _appDBContext = appDBContext;
        }
        public T Add(T entity)
        {
           T result = _appDBContext.Add(entity).Entity;
            _appDBContext.SaveChanges();
            return result;
        }

        public async Task<T> AddAsync(T entity, CancellationToken ct)
        {
            EntityEntry<T> result = await _appDBContext.AddAsync(entity, ct);
            await _appDBContext.SaveChangesAsync(ct);
            //Task<T> df =  result.Entity;
            return result.Entity;
        } 

        public void Delete(T entity)
        {
            _appDBContext.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            _appDBContext.Set<T>().RemoveRange(entities);
        }

        public T Find(Expression<Func<T, bool>> predicate)
        {
            return _appDBContext.Set<T>().SingleOrDefault(predicate);
        }

        public T Find(Expression<Func<T, bool>> predicate, string[] includes= null)
        {
            IQueryable<T> query = _appDBContext.Set<T>();  
            if (includes != null) 
            {
              foreach (string include in includes) 
                    query = query.Include(include);
            
            }
            return query.SingleOrDefault(predicate);
            //   throw new NotImplementedException();
        }

        public IEnumerable<T> FindAll(Expression<Func<T, bool>> predicate, string[] includes=null)
        {
            IQueryable<T> query = _appDBContext.Set<T>();
            if (includes != null)
            {
                foreach (string include in includes)
                    query = query.Include(include);

            }
            return query.Where(predicate);

        }

        public IEnumerable<T> Findskiptake(Expression<Func<T, bool>> predicate, int? take =null, int? skip =null)
        {
            if (skip != null && take != null)
                return _appDBContext.Set<T>().Where(predicate).Skip((int)skip).Take((int)take);
            else
                return _appDBContext.Set<T>().Where(predicate);
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            IEnumerable<T> values = _appDBContext.Set<T>().ToList();
           // throw new NotImplementedException();
           return values;
        }

        public T GetById(long id)
        {
            return _appDBContext.Set<T>().Find(id);
        }

        public T Update(T entity)
        {
            return _appDBContext.Update(entity).Entity;
        }
    }
}
