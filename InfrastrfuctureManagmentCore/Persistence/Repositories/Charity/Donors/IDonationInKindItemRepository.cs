using InfrastrfuctureManagmentCore.Domains.Charity.Donors;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors
{
    public interface IDonationInKindItemRepository
    {
        Task<List<DonationInKindItem>> GetByDonationIdAsync(Guid donationId);
        Task<DonationInKindItem?> GetByIdAsync(Guid id);
        Task<decimal> GetAllocatedQuantityAsync(Guid donationInKindItemId, Guid? excludeAllocationId = null);
        Task AddAsync(DonationInKindItem item);
        Task UpdateAsync(DonationInKindItem item);
    }
}
