using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class DonationAllocationRepository : IDonationAllocationRepository
    {
        private readonly AppDbContext _db;

        public DonationAllocationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<DonationAllocation>> GetByDonationIdAsync(Guid donationId)
        {
            return await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Include(x => x.AidRequest)
                    .ThenInclude(x => x!.AidType)
                .Include(x => x.Beneficiary)
                .Include(x => x.DonationInKindItem)
                    .ThenInclude(x => x!.Item)
                .Where(x => x.DonationId == donationId)
                .OrderByDescending(x => x.AllocatedDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<DonationAllocation?> GetByIdAsync(Guid id)
        {
            return await _db.Set<DonationAllocation>()
                .Include(x => x.Donation)
                    .ThenInclude(x => x!.AidType)
                .Include(x => x.AidRequest)
                    .ThenInclude(x => x!.AidType)
                .Include(x => x.Beneficiary)
                .Include(x => x.DonationInKindItem)
                    .ThenInclude(x => x!.Item)
                .Include(x => x.DisbursementFundingLines)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<decimal> GetAllocatedAmountAsync(Guid donationId, Guid? excludeAllocationId = null)
        {
            var query = _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.DonationId == donationId && x.ApprovalStatus != "Rejected");

            if (excludeAllocationId.HasValue)
                query = query.Where(x => x.Id != excludeAllocationId.Value);

            return await query.SumAsync(x => x.Amount ?? 0m);
        }

        public async Task AddAsync(DonationAllocation allocation)
        {
            await _db.Set<DonationAllocation>().AddAsync(allocation);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(DonationAllocation allocation)
        {
            _db.Set<DonationAllocation>().Update(allocation);
            await _db.SaveChangesAsync();
        }
    }
}
