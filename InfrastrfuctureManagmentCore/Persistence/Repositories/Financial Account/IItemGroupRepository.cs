using InfrastrfuctureManagmentCore.Domains.Item;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account
{
    public interface IItemGroupRepository
    {
        Task<List<ItemGroup>> GetAllAsync();
        Task<List<ItemGroup>> GetActiveAsync();
        Task<List<ItemGroup>> GetParentsAsync();

        Task<ItemGroup?> GetByIdAsync(Guid id);
        Task<ItemGroup?> GetByCodeAsync(string code);

        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);
        Task<bool> HasActiveChildrenAsync(Guid groupId);

        Task AddAsync(ItemGroup group);
        Task UpdateAsync(ItemGroup group);
    }
}
