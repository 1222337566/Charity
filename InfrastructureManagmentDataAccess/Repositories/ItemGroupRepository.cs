using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Item;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class ItemGroupRepository : IItemGroupRepository
    {
        private readonly AppDbContext _db;

        public ItemGroupRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ItemGroup>> GetAllAsync()
        {
            return await _db.ItemGroups
                .AsNoTracking()
                .Include(x => x.ParentGroup)
                .OrderBy(x => x.GroupCode)
                .ToListAsync();
        }

        public async Task<List<ItemGroup>> GetActiveAsync()
        {
            return await _db.ItemGroups
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.GroupCode)
                .ToListAsync();
        }

        public async Task<List<ItemGroup>> GetParentsAsync()
        {
            return await _db.ItemGroups
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.GroupCode)
                .ToListAsync();
        }

        public async Task<ItemGroup?> GetByIdAsync(Guid id)
        {
            return await _db.ItemGroups
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<ItemGroup?> GetByCodeAsync(string code)
        {
            return await _db.ItemGroups
                .FirstOrDefaultAsync(x => x.GroupCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _db.ItemGroups
                .AnyAsync(x => x.GroupCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return await _db.ItemGroups
                .AnyAsync(x => x.GroupCode == code && x.Id != excludeId);
        }

        public async Task<bool> HasActiveChildrenAsync(Guid groupId)
        {
            return await _db.ItemGroups
                .AnyAsync(x => x.ParentGroupId == groupId && x.IsActive);
        }

        public async Task AddAsync(ItemGroup group)
        {
            await _db.ItemGroups.AddAsync(group);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ItemGroup group)
        {
            _db.ItemGroups.Update(group);
            await _db.SaveChangesAsync();
        }
    }
}
