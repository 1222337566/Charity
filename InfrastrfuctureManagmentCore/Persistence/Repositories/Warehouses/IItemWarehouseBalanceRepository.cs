using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses
{
    public interface IItemWarehouseBalanceRepository
    {
        Task<List<ItemWarehouseBalance>> GetAllAsync();
        Task<ItemWarehouseBalance?> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId);
        Task<List<ItemWarehouseBalance>> GetByWarehouseAsync(Guid warehouseId);
        Task AddAsync(ItemWarehouseBalance balance);
        Task UpdateAsync(ItemWarehouseBalance balance);
    }
}
