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

    

    public class UnitRepository : IUnitRepository
    {
        private readonly AppDbContext _db;

        public UnitRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Unit>> GetAllAsync()
        {
            return await _db.Units
                .AsNoTracking()
                .OrderBy(x => x.UnitCode)
                .ToListAsync();
        }

        public async Task<List<Unit>> GetActiveAsync()
        {
            return await _db.Units
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.UnitCode)
                .ToListAsync();
        }

        public async Task<Unit?> GetByIdAsync(Guid id)
        {
            return await _db.Units
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Unit?> GetByCodeAsync(string code)
        {
            return await _db.Units
                .FirstOrDefaultAsync(x => x.UnitCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _db.Units
                .AnyAsync(x => x.UnitCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return await _db.Units
                .AnyAsync(x => x.UnitCode == code && x.Id != excludeId);
        }

        public async Task AddAsync(Unit unit)
        {
            await _db.Units.AddAsync(unit);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Unit unit)
        {
            _db.Units.Update(unit);
            await _db.SaveChangesAsync();
        }
    }
}
