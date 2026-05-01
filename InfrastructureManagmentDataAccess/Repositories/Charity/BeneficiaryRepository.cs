using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class BeneficiaryRepository : IBeneficiaryRepository
    {
        private readonly AppDbContext _db;

        public BeneficiaryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Beneficiary>> SearchAsync(string? q, Guid? statusId, bool? isActive)
        {
            var query = _db.Beneficiaries
                .AsNoTracking()
                .Include(x => x.Status)
                .Include(x => x.Gender)
                .Include(x => x.Governorate)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.Code.Contains(q) ||
                    x.FullName.Contains(q) ||
                    (x.NationalId != null && x.NationalId.Contains(q)) ||
                    (x.PhoneNumber != null && x.PhoneNumber.Contains(q)) ||
                    (x.AlternatePhoneNumber != null && x.AlternatePhoneNumber.Contains(q)));
            }

            if (statusId.HasValue && statusId.Value != Guid.Empty)
                query = query.Where(x => x.StatusId == statusId.Value);

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<Beneficiary?> GetByIdAsync(Guid id)
        {
            return await _db.Beneficiaries
                .Include(x => x.Status)
                .Include(x => x.Gender)
                .Include(x => x.MaritalStatus)
                .Include(x => x.Governorate)
                .Include(x => x.City)
                .Include(x => x.Area)
                .Include(x => x.FamilyMembers)
                .Include(x => x.Documents)
                .Include(x => x.Assessments).ThenInclude(x => x.RecommendedAidType)
                .Include(x => x.CommitteeDecisions).ThenInclude(x => x.ApprovedAidType)
                .Include(x => x.OldRecords)
                .Include(x => x.AidRequests).ThenInclude(x => x.AidType)
                .Include(x => x.AidDisbursements).ThenInclude(x => x.AidType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code)
            => await _db.Beneficiaries.AnyAsync(x => x.Code == code);

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
            => await _db.Beneficiaries.AnyAsync(x => x.Code == code && x.Id != excludeId);

        public async Task<bool> NationalIdExistsAsync(string nationalId)
            => await _db.Beneficiaries.AnyAsync(x => x.NationalId == nationalId);

        public async Task<bool> NationalIdExistsAsync(string nationalId, Guid excludeId)
            => await _db.Beneficiaries.AnyAsync(x => x.NationalId == nationalId && x.Id != excludeId);

        public async Task AddAsync(Beneficiary beneficiary)
        {
            await _db.Beneficiaries.AddAsync(beneficiary);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Beneficiary beneficiary)
        {
            _db.Beneficiaries.Update(beneficiary);
            await _db.SaveChangesAsync();
        }
    }
}
