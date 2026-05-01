using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class DonationRepository : IDonationRepository
    {
        private readonly AppDbContext _db;

        public DonationRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Donation>> GetByDonorIdAsync(Guid donorId)
        {
            return await _db.Set<Donation>()
                .AsNoTracking()
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .Where(x => x.DonorId == donorId)
                .OrderByDescending(x => x.DonationDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<Donation?> GetByIdAsync(Guid id)
        {
            return await _db.Set<Donation>()
                .Include(x => x.Donor)
                .Include(x => x.AidType)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> DonationNumberExistsAsync(string donationNumber)
            => await _db.Set<Donation>().AnyAsync(x => x.DonationNumber == donationNumber);

        public async Task<bool> DonationNumberExistsAsync(string donationNumber, Guid excludeId)
            => await _db.Set<Donation>().AnyAsync(x => x.DonationNumber == donationNumber && x.Id != excludeId);

        public async Task AddAsync(Donation donation)
        {
            await _db.Set<Donation>().AddAsync(donation);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Donation donation)
        {
            _db.Set<Donation>().Update(donation);
            await _db.SaveChangesAsync();
        }
    }
}
