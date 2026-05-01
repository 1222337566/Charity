using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class PersonalInformationRepository : IPersonalInformationRepository
    {
        protected readonly AppDbContext _appDBContext;
        public PersonalInformationRepository(AppDbContext appDBContext)
        {

            _appDBContext = appDBContext;
        }
        public PersonalInformation Add(PersonalInformation entity)
        {
            PersonalInformation result = _appDBContext.Add(entity).Entity;
            _appDBContext.SaveChanges();
            return result;
        }

        public void Delete(PersonalInformation entity)
        {
            _appDBContext.Set<PersonalInformation>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<PersonalInformation> entities)
        {
            _appDBContext.Set<PersonalInformation>().RemoveRange(entities);
        }

        public PersonalInformation Find(Expression<Func<PersonalInformation, bool>> predicate)
        {
            return _appDBContext.Set<PersonalInformation>().SingleOrDefault(predicate);
        }

        public PersonalInformation Find(Expression<Func<PersonalInformation, bool>> predicate, string[] includes = null)
        {
            IQueryable<PersonalInformation> query = _appDBContext.Set<PersonalInformation>();
            if (includes != null)
            {
                foreach (string include in includes)
                    query = query.Include(include);

            }
            return query.SingleOrDefault(predicate);
            //   throw new NotImplementedException();
        }

        public IEnumerable<PersonalInformation> FindAll(Expression<Func<PersonalInformation, bool>> predicate, string[] includes = null)
        {
            IQueryable<PersonalInformation> query = _appDBContext.Set<PersonalInformation>();
            if (includes != null)
            {
                foreach (string include in includes)
                    query = query.Include(include);

            }
            return query.Where(predicate);

        }

        public IEnumerable<PersonalInformation> Findskiptake(Expression<Func<PersonalInformation, bool>> predicate, int? take = null, int? skip = null)
        {
            if (skip != null && take != null)
                return _appDBContext.Set<PersonalInformation>().Where(predicate).Skip((int)skip).Take((int)take);
            else
                return _appDBContext.Set<PersonalInformation>().Where(predicate);
        }

        public async Task<IEnumerable<PersonalInformation>> GetAll()
        {
            IEnumerable<PersonalInformation> values = _appDBContext.Set<PersonalInformation>().ToList();
            // throw new NotImplementedException();
            return values;
        }

        public PersonalInformation GetById(int id)
        {
            return _appDBContext.Set<PersonalInformation>().Find(id);
        }

        public PersonalInformation Update(PersonalInformation entity)
        {
            return _appDBContext.Update(entity).Entity;
        }

        public Task<PersonalInformation> GetByUserIdAsync(string userId, CancellationToken ct = default)
    => _appDBContext.PersonalInformation.FirstOrDefaultAsync(x => x.UserId == userId, ct);


    }
}
