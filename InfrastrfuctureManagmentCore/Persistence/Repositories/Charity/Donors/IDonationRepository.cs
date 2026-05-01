using InfrastrfuctureManagmentCore.Domains.Charity.Donors;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors
{
    public interface IDonationRepository
    {
        Task<List<Donation>> GetByDonorIdAsync(Guid donorId);
        Task<Donation?> GetByIdAsync(Guid id);
        Task<bool> DonationNumberExistsAsync(string donationNumber);
        Task<bool> DonationNumberExistsAsync(string donationNumber, Guid excludeId);
        Task AddAsync(Donation donation);
        Task UpdateAsync(Donation donation);
    }
}
