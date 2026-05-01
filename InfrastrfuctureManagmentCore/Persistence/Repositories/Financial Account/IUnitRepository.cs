using InfrastrfuctureManagmentCore.Domains.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account
{
    public interface IUnitRepository
    {
        Task<List<Unit>> GetAllAsync();
        Task<List<Unit>> GetActiveAsync();
        Task<Unit?> GetByIdAsync(Guid id);
        Task<Unit?> GetByCodeAsync(string code);

        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);

        Task AddAsync(Unit unit);
        Task UpdateAsync(Unit unit);
    }
}
