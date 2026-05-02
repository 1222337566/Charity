using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.AidCycles
{
    public class AidCycleGenerationService : IAidCycleGenerationService
    {
        private readonly AppDbContext _db;

        public AidCycleGenerationService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AidCycleGenerationResult> GenerateAsync(Guid aidCycleId, bool clearExistingDraftLines = false)
        {
            var result = new AidCycleGenerationResult();

            var cycle = await _db.Set<AidCycle>().FirstOrDefaultAsync(x => x.Id == aidCycleId);
            if (cycle == null)
            {
                result.Messages.Add("الدورة غير موجودة");
                return result;
            }

            if (cycle.Status is "Closed" or "Cancelled")
            {
                result.Messages.Add("لا يمكن التوليد لدورة مغلقة أو ملغاة");
                return result;
            }

            if (clearExistingDraftLines)
            {
                var oldLines = await _db.Set<AidCycleBeneficiary>()
                    .Where(x => x.AidCycleId == aidCycleId && x.Status != "Disbursed")
                    .ToListAsync();
                _db.Set<AidCycleBeneficiary>().RemoveRange(oldLines);
                await _db.SaveChangesAsync();
            }

            var existingIds = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == aidCycleId)
                .Select(x => x.BeneficiaryId)
                .ToListAsync();

            var decisions = await _db.Set<BeneficiaryCommitteeDecision>()
                .Include(x => x.Beneficiary)
                .Where(x => x.Beneficiary != null
                            && x.Beneficiary.IsActive
                            && x.ApprovedAmount != null
                            && x.ApprovedAmount > 0
                            && (cycle.AidTypeId == null || x.ApprovedAidTypeId == cycle.AidTypeId || x.ApprovedAidTypeId == null))
                .OrderByDescending(x => x.DecisionDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

            var latestDecisions = decisions
                .GroupBy(x => x.BeneficiaryId)
                .Select(g => g.First())
                .Where(x => x.ApprovedStatus && !existingIds.Contains(x.BeneficiaryId))
                .ToList();

            var newLines = new List<AidCycleBeneficiary>();
            foreach (var decision in latestDecisions)
            {
                var effectiveAidTypeId = decision.ApprovedAidTypeId ?? cycle.AidTypeId;
                if (!effectiveAidTypeId.HasValue)
                    continue;

                var lastDisbursementDate = await _db.Set<BeneficiaryAidDisbursement>()
                    .Where(x => x.BeneficiaryId == decision.BeneficiaryId && x.AidTypeId == effectiveAidTypeId.Value)
                    .OrderByDescending(x => x.DisbursementDate)
                    .Select(x => (DateTime?)x.DisbursementDate)
                    .FirstOrDefaultAsync();

                newLines.Add(new AidCycleBeneficiary
                {
                    Id = Guid.NewGuid(),
                    AidCycleId = cycle.Id,
                    BeneficiaryId = decision.BeneficiaryId,
                    CommitteeDecisionId = decision.Id,
                    AidTypeId = effectiveAidTypeId.Value,
                    ScheduledAmount = decision.ApprovedAmount,
                    ApprovedAmount = decision.ApprovedAmount,
                    Status = "Eligible",
                    LastDisbursementDate = lastDisbursementDate,
                    NextDueDate = cycle.PlannedDisbursementDate.Date,
                    Notes = decision.CommitteeNotes,
                    CreatedAtUtc = DateTime.UtcNow
                });
            }

            if (newLines.Count > 0)
            {
                await _db.Set<AidCycleBeneficiary>().AddRangeAsync(newLines);
                cycle.Status = "Generated";
                cycle.BeneficiariesCount += newLines.Count;
                cycle.TotalPlannedAmount = (cycle.TotalPlannedAmount ?? 0m) + newLines.Sum(x => x.ApprovedAmount ?? x.ScheduledAmount ?? 0m);
                await _db.SaveChangesAsync();
            }

            result.AddedCount = newLines.Count;
            result.TotalAmount = newLines.Sum(x => x.ApprovedAmount ?? x.ScheduledAmount ?? 0m);
            if (newLines.Count == 0)
                result.Messages.Add("لم يتم العثور على مستحقين جدد للدورة");

            return result;
        }
    }
}
