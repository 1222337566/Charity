using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class DonorRepository : IDonorRepository
    {
        private readonly AppDbContext _db;

        public DonorRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Donor>> SearchAsync(string? q, string? donorType, bool? isActive)
        {
            var query = _db.Set<Donor>()
                .AsNoTracking()
                .Include(x => x.Governorate)
                .Include(x => x.City)
                .Include(x => x.Area)
                .Include(x => x.Donations)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.Code.Contains(q) ||
                    x.FullName.Contains(q) ||
                    (x.ContactPerson != null && x.ContactPerson.Contains(q)) ||
                    (x.NationalIdOrTaxNo != null && x.NationalIdOrTaxNo.Contains(q)) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(q)) ||
                    (x.Email != null && x.Email.Contains(q)));
            }

            if (!string.IsNullOrWhiteSpace(donorType))
                query = query.Where(x => x.DonorType == donorType);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query.OrderByDescending(x => x.CreatedAtUtc).ToListAsync();
        }

        public async Task<Donor?> GetByIdAsync(Guid id)
        {
            return await _db.Set<Donor>()
                .Include(x => x.Governorate)
                .Include(x => x.City)
                .Include(x => x.Area)
                .Include(x => x.Donations.OrderByDescending(d => d.DonationDate))
                    .ThenInclude(d => d.PaymentMethod)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code)
            => await _db.Set<Donor>().AnyAsync(x => x.Code == code);

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
            => await _db.Set<Donor>().AnyAsync(x => x.Code == code && x.Id != excludeId);

        public async Task<bool> NationalIdOrTaxNoExistsAsync(string value)
            => await _db.Set<Donor>().AnyAsync(x => x.NationalIdOrTaxNo == value);

        public async Task<bool> NationalIdOrTaxNoExistsAsync(string value, Guid excludeId)
            => await _db.Set<Donor>().AnyAsync(x => x.NationalIdOrTaxNo == value && x.Id != excludeId);

        public async Task AddAsync(Donor donor)
        {
            await _db.Set<Donor>().AddAsync(donor);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Donor donor)
        {
            _db.Set<Donor>().Update(donor);
            await _db.SaveChangesAsync();
        }
    }
}
