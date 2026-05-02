using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class DonationInKindItemRepository : IDonationInKindItemRepository
    {
        private readonly AppDbContext _db;

        public DonationInKindItemRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<DonationInKindItem>> GetByDonationIdAsync(Guid donationId)
        {
            return await _db.Set<DonationInKindItem>()
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .Where(x => x.DonationId == donationId)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<DonationInKindItem?> GetByIdAsync(Guid id)
        {
            return await _db.Set<DonationInKindItem>()
                .Include(x => x.Donation)
                .Include(x => x.Item)
                .Include(x => x.Warehouse)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<decimal> GetAllocatedQuantityAsync(Guid donationInKindItemId, Guid? excludeAllocationId = null)
        {
            var query = _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.DonationInKindItemId == donationInKindItemId);

            if (excludeAllocationId.HasValue)
                query = query.Where(x => x.Id != excludeAllocationId.Value);

            return await query.SumAsync(x => x.AllocatedQuantity ?? 0m);
        }

        public async Task AddAsync(DonationInKindItem item)
        {
            await _db.Set<DonationInKindItem>().AddAsync(item);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(DonationInKindItem item)
        {
            _db.Set<DonationInKindItem>().Update(item);
            await _db.SaveChangesAsync();
        }
    }
}
