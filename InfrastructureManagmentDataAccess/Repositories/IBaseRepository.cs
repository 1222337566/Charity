using InfrastructureManagmentCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public interface IBaseRepository<T> where T : BaseEntity
    {
        T Add(T entity);
        Task<IEnumerable<T>> GetAll();
        Task<T> AddAsync(T entity,CancellationToken ct);
        T GetById(long id);
        T Find(Expression<Func<T, bool>> predicate);
        T Find(Expression<Func<T, bool>> predicate,string[] includes =null);
        IEnumerable<T> FindAll(Expression<Func<T,bool>> predicate ,string[] includes=null);
        IEnumerable<T> Findskiptake(Expression<Func<T, bool>> predicate, int? take =null, int? skip = null);
        public void Delete (T entity);
        public T Update (T entity);
        public void DeleteRange(IEnumerable<T> entities);

            
    }
}
