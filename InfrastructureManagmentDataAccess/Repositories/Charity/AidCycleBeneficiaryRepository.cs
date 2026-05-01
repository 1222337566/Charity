using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.AidCycles;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class AidCycleBeneficiaryRepository : IAidCycleBeneficiaryRepository
    {
        private readonly AppDbContext _db;

        public AidCycleBeneficiaryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IReadOnlyList<AidCycleEligibleBeneficiaryVm>> GetEligibleBeneficiariesForAddAsync(Guid aidCycleId)
        {
            var cycle = await _db.Set<AidCycle>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == aidCycleId);

            if (cycle == null)
                return Array.Empty<AidCycleEligibleBeneficiaryVm>();

            var existingIds = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == aidCycleId)
                .Select(x => x.BeneficiaryId)
                .ToListAsync();

            var committeeDecisions = await _db.Set<BeneficiaryCommitteeDecision>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.Beneficiary!.Status)
                .Include(x => x.ApprovedAidType)
                .Where(x => !cycle.AidTypeId.HasValue || x.ApprovedAidTypeId == cycle.AidTypeId.Value || x.ApprovedAidTypeId == null)
                .ToListAsync();

            var latestApprovedPerBeneficiary = committeeDecisions
                .GroupBy(x => x.BeneficiaryId)
                .Select(g => g
                    .OrderByDescending(x => x.DecisionDate)
                    .ThenByDescending(x => x.CreatedAtUtc)
                    .First())
                .Where(x => x.ApprovedStatus && !existingIds.Contains(x.BeneficiaryId));

            return latestApprovedPerBeneficiary
                .OrderBy(x => x.Beneficiary!.FullName)
                .Select(x => new AidCycleEligibleBeneficiaryVm
                {
                    BeneficiaryId = x.BeneficiaryId,
                    Code = x.Beneficiary?.Code,
                    FullName = x.Beneficiary?.FullName,
                    NationalId = x.Beneficiary?.NationalId,
                    PhoneNumber = x.Beneficiary?.PhoneNumber,
                    StatusName = x.Beneficiary?.Status?.NameAr,
                    ApprovedAmount = x.ApprovedAmount,
                    AidType = x.ApprovedAidType?.NameAr,
                    CommitteeDecisionId = x.Id
                })
                .ToList();
        }

        public async Task<int> AddBeneficiariesAsync(Guid aidCycleId, IEnumerable<Guid> beneficiaryIds, decimal? approvedAmount, string? notes, ClaimsPrincipal user)
        {
            var cycle = await _db.Set<AidCycle>()
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == aidCycleId);

            if (cycle == null)
                throw new InvalidOperationException("الدورة غير موجودة أو تم حذفها.");

            var distinctIds = beneficiaryIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            if (!distinctIds.Any())
                return 0;

            var existingIds = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == aidCycleId && distinctIds.Contains(x.BeneficiaryId))
                .Select(x => x.BeneficiaryId)
                .ToListAsync();

            var toAdd = distinctIds.Except(existingIds).ToList();
            if (!toAdd.Any())
                return 0;

            var committeeDecisions = await _db.Set<BeneficiaryCommitteeDecision>()
                .AsNoTracking()
                .Where(x => toAdd.Contains(x.BeneficiaryId)
                            && (!cycle.AidTypeId.HasValue || x.ApprovedAidTypeId == cycle.AidTypeId.Value || x.ApprovedAidTypeId == null))
                .ToListAsync();

            var latestDecisionMap = committeeDecisions
                .GroupBy(x => x.BeneficiaryId)
                .Select(g => g.OrderByDescending(x => x.DecisionDate).ThenByDescending(x => x.CreatedAtUtc).First())
                .Where(x => x.ApprovedStatus)
                .ToDictionary(x => x.BeneficiaryId, x => x);

            var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var added = 0;

            foreach (var beneficiaryId in toAdd)
            {
                if (!latestDecisionMap.TryGetValue(beneficiaryId, out var decision))
                    continue;

                if (cycle.AidTypeId.HasValue && decision.ApprovedAidTypeId.HasValue && decision.ApprovedAidTypeId != cycle.AidTypeId.Value)
                    continue;

                var effectiveAidTypeId = decision.ApprovedAidTypeId ?? cycle.AidTypeId;
                if (!effectiveAidTypeId.HasValue)
                    continue;

                var effectiveAmount = approvedAmount ?? decision.ApprovedAmount;

                _db.Set<AidCycleBeneficiary>().Add(new AidCycleBeneficiary
                {
                    Id = Guid.NewGuid(),
                    AidCycleId = aidCycleId,
                    BeneficiaryId = beneficiaryId,
                    CommitteeDecisionId = decision.Id,
                    AidTypeId = effectiveAidTypeId.Value,
                    ScheduledAmount = effectiveAmount,
                    ApprovedAmount = effectiveAmount,
                    Notes = notes,
                    Status = "Eligible",
                    NextDueDate = cycle.PlannedDisbursementDate.Date,
                    CreatedByUserId = userId,
                    CreatedAtUtc = DateTime.UtcNow,
                });

                added++;
            }

            if (added > 0)
            {
                await _db.SaveChangesAsync();
            }

            return added;
        }

        public async Task<List<AidCycleBeneficiary>> GetByCycleIdAsync(Guid cycleId)
        {
            return await _db.Set<AidCycleBeneficiary>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Where(x => x.AidCycleId == cycleId)
                .OrderBy(x => x.Beneficiary!.Code)
                .ToListAsync();
        }

        public async Task<AidCycleBeneficiary?> GetByIdAsync(Guid id)
        {
            return await _db.Set<AidCycleBeneficiary>()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.AidCycle)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddRangeAsync(IEnumerable<AidCycleBeneficiary> entities)
        {
            await _db.Set<AidCycleBeneficiary>().AddRangeAsync(entities);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(AidCycleBeneficiary entity)
        {
            _db.Set<AidCycleBeneficiary>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<List<AidCycleBeneficiary>> GetDueListAsync(DateTime? dueDate = null)
        {
            var target = (dueDate ?? DateTime.Today).Date;
            return await _db.Set<AidCycleBeneficiary>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)
                .Include(x => x.AidType)
                .Include(x => x.AidCycle)
                .Where(x => x.Status != "Cancelled" && x.Status != "Stopped" && x.NextDueDate != null && x.NextDueDate <= target)
                .OrderBy(x => x.NextDueDate)
                .ThenBy(x => x.Beneficiary!.Code)
                .ToListAsync();
        }
    }
}
