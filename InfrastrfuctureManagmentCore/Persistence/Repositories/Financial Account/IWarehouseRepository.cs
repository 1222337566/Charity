using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account
{
    public interface IWarehouseRepository
    {
        Task<List<Warehouse>> GetAllAsync();
        Task<List<Warehouse>> GetActiveAsync();

        Task<Warehouse?> GetByIdAsync(Guid id);
        Task<Warehouse?> GetByCodeAsync(string code);

        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);

        Task AddAsync(Warehouse warehouse);
        Task UpdateAsync(Warehouse warehouse);
    }
}
