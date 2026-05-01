using InfrastructureManagmentCore.Domains.Connection;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public interface IPersonalInformationRepository 
        {
        PersonalInformation Add(PersonalInformation entity);
        Task<IEnumerable<PersonalInformation>> GetAll();

        PersonalInformation GetById(int id);
        PersonalInformation Find(Expression<Func<PersonalInformation, bool>> predicate);
        PersonalInformation Find(Expression<Func<PersonalInformation, bool>> predicate, string[] includes = null);
        IEnumerable<PersonalInformation> FindAll(Expression<Func<PersonalInformation, bool>> predicate, string[] includes = null);
        IEnumerable<PersonalInformation> Findskiptake(Expression<Func<PersonalInformation, bool>> predicate, int? take = null, int? skip = null);
        public void Delete(PersonalInformation entity);
        public PersonalInformation Update(PersonalInformation entity);
        public void DeleteRange(IEnumerable<PersonalInformation> entities);
        Task<PersonalInformation> GetByUserIdAsync(string userId, CancellationToken ct = default);






    }
}
