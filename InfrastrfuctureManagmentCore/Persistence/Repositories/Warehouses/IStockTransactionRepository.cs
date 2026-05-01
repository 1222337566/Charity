using InfrastrfuctureManagmentCore.Domains.Warehouses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses
{
    public interface IStockTransactionRepository
    {
        Task AddAsync(StockTransaction transaction);
        Task<List<StockTransaction>> GetByItemAsync(Guid itemId);
        Task<List<StockTransaction>> GetByWarehouseAsync(Guid warehouseId);
        Task<List<StockTransaction>> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId);
    }
}
