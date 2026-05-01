using InfrastrfuctureManagmentCore.Domains.Charity.Donors;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors
{
    public interface IDonationAllocationRepository
    {
        Task<List<DonationAllocation>> GetByDonationIdAsync(Guid donationId);
        Task<DonationAllocation?> GetByIdAsync(Guid id);
        Task<decimal> GetAllocatedAmountAsync(Guid donationId, Guid? excludeAllocationId = null);
        Task AddAsync(DonationAllocation allocation);
        Task UpdateAsync(DonationAllocation allocation);
    }
}
