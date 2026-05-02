using InfrastrfuctureManagmentCore.Domains.Financial;
using InfrastructureManagmentCore.Domains.Profiling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account
{
    public interface IAccountRepository
    {
        Task<List<FinancialAccount>> GetAllAsync();
        Task<FinancialAccount?> GetByIdAsync(Guid id);
        Task<FinancialAccount?> GetByCodeAsync(string accountCode);
        Task<List<FinancialAccount>> GetParentsAsync();
        Task AddAsync(FinancialAccount account);
        Task UpdateAsync(FinancialAccount account);
        Task<List<FinancialAccount>> GetAllParentsAsync();
        Task<bool> AccountCodeExistsAsync(string accountCode);
        Task<bool> HasChildrenAsync(Guid accountId);
        Task<bool> AccountCodeExistsAsync(string accountCode, Guid excludeId);
        Task<bool> HasActiveChildrenAsync(Guid accountId);
        Task SaveChangesAsync();
    }
}
