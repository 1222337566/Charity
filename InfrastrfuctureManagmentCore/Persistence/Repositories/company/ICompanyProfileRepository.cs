using InfrastrfuctureManagmentCore.Domains.Company;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.company
{
    public interface ICompanyProfileRepository
    {
        Task<CompanyProfile?> GetActiveAsync();
        Task<CompanyProfile?> GetByIdAsync(Guid id);
        Task AddAsync(CompanyProfile profile);
        Task UpdateAsync(CompanyProfile profile);
    }
}
