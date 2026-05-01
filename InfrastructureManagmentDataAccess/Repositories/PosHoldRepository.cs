using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.PosHolds;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.PosHolds;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class PosHoldRepository : IPosHoldRepository
    {
        private readonly AppDbContext _db;

        public PosHoldRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PosHold>> GetHeldAsync()
        {
            return await _db.PosHolds
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.PaymentMethod)
                .Where(x => x.Status == PosHoldStatus.Held)
                .OrderByDescending(x => x.HoldDateUtc)
                .ToListAsync();
        }

        public async Task<PosHold?> GetByIdAsync(Guid id)
        {
            return await _db.PosHolds
                .Include(x => x.Warehouse)
                .Include(x => x.PaymentMethod)
                .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> HoldNumberExistsAsync(string holdNumber)
        {
            return await _db.PosHolds.AnyAsync(x => x.HoldNumber == holdNumber);
        }

        public async Task AddAsync(PosHold hold)
        {
            await _db.PosHolds.AddAsync(hold);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(PosHold hold)
        {
            _db.PosHolds.Update(hold);
            await _db.SaveChangesAsync();
        }
    }
}
