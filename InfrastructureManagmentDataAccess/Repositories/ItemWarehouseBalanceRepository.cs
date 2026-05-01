using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Warehouses;
   // using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouse;
   // using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouse;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Warehouses;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class ItemWarehouseBalanceRepository : IItemWarehouseBalanceRepository
    {
        private readonly AppDbContext _db;

        public ItemWarehouseBalanceRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ItemWarehouseBalance>> GetAllAsync()
        {
            return await _db.ItemWarehouseBalances
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .OrderBy(x => x.Item!.ItemCode)
                .ToListAsync();
        }

        public async Task<ItemWarehouseBalance?> GetByItemAndWarehouseAsync(Guid itemId, Guid warehouseId)
        {
            return await _db.ItemWarehouseBalances
                .FirstOrDefaultAsync(x => x.ItemId == itemId && x.WarehouseId == warehouseId);
        }

        public async Task<List<ItemWarehouseBalance>> GetByWarehouseAsync(Guid warehouseId)
        {
            return await _db.ItemWarehouseBalances
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.WarehouseId == warehouseId)
                .OrderBy(x => x.Item!.ItemCode)
                .ToListAsync();
        }

        public async Task AddAsync(ItemWarehouseBalance balance)
        {
            await _db.ItemWarehouseBalances.AddAsync(balance);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ItemWarehouseBalance balance)
        {
            _db.ItemWarehouseBalances.Update(balance);
            await _db.SaveChangesAsync();
        }
    }
}
