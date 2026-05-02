using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Warehouses;
    //using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouse;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class StockTransactionRepository : IStockTransactionRepository
    {
        private readonly AppDbContext _db;

        public StockTransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(StockTransaction transaction)
        {
            await _db.StockTransactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
        }

        public async Task<List<StockTransaction>> GetByItemAsync(Guid itemId)
        {
            return await _db.StockTransactions
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.ItemId == itemId)
                .OrderByDescending(x => x.TransactionDateUtc)
                .ToListAsync();
        }

        public async Task<List<StockTransaction>> GetByWarehouseAsync(Guid warehouseId)
        {
            return await _db.StockTransactions
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.WarehouseId == warehouseId)
                .OrderByDescending(x => x.TransactionDateUtc)
                .ToListAsync();
        }

        public async Task<List<StockTransaction>> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId)
        {
            return await _db.StockTransactions
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.ItemId == itemId && x.WarehouseId == warehouseId)
                .OrderByDescending(x => x.TransactionDateUtc)
                .ToListAsync();
        }
    }
}
