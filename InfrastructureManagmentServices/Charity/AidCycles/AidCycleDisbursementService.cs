using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using InfrastructureManagmentServices.Charity.Beneficiaries;

namespace InfrastructureManagmentServices.Charity.AidCycles
{
    public class AidCycleDisbursementService : IAidCycleDisbursementService
    {
        private readonly AppDbContext _db;

        public AidCycleDisbursementService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AidCycleDisbursementResult> DisburseAsync(AidCycleDisbursementRequest request)
        {
            var result = new AidCycleDisbursementResult();
            var cycle = await _db.Set<AidCycle>().FirstOrDefaultAsync(x => x.Id == request.AidCycleId);
            if (cycle == null)
            {
                result.Messages.Add("الدورة غير موجودة");
                return result;
            }

            if (cycle.Status is "Closed" or "Cancelled")
            {
                result.Messages.Add("لا يمكن الصرف من دورة مغلقة أو ملغاة");
                return result;
            }

            if (cycle.Status != "Approved")
            {
                result.Messages.Add("يجب اعتماد دورة الصرف أولاً قبل تنفيذ الصرف.");
                return result;
            }

            var lines = await _db.Set<AidCycleBeneficiary>()
                .Where(x => x.AidCycleId == request.AidCycleId && request.LineIds.Contains(x.Id))
                .ToListAsync();

            foreach (var line in lines.Where(x => x.Status != "Disbursed"))
            {
                var amount = line.ApprovedAmount ?? line.ScheduledAmount;
                if (amount == null || amount <= 0)
                    continue;

                var validation = await BeneficiaryCommitteeDecisionGuard.ValidateAsync(
                    _db,
                    line.BeneficiaryId,
                    line.AidTypeId,
                    amount,
                    "صرف المستفيد من دورة الصرف",
                    line.CommitteeDecisionId);

                if (!validation.IsValid)
                {
                    result.Messages.Add(validation.Message);
                    continue;
                }

                var disbursement = new BeneficiaryAidDisbursement
                {
                    Id = Guid.NewGuid(),
                    BeneficiaryId = line.BeneficiaryId,
                    AidTypeId = line.AidTypeId,
                    DisbursementDate = request.DisbursementDate,
                    Amount = amount,
                    PaymentMethodId = request.PaymentMethodId,
                    FinancialAccountId = request.FinancialAccountId,
                    Notes = string.IsNullOrWhiteSpace(request.Notes) ? $"صرف من دورة {cycle.CycleNumber}" : request.Notes,
                    ApprovalStatus = AidDisbursementApprovalStatusCodes.Approved,
                    ApprovedByUserId = request.UserId,
                    ApprovedAtUtc = DateTime.UtcNow,
                    RejectedByUserId = null,
                    RejectedAtUtc = null,
                    CreatedByUserId = request.UserId,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _db.Set<BeneficiaryAidDisbursement>().AddAsync(disbursement);

                line.DisbursementId = disbursement.Id;
                line.DisbursedAmount = amount;
                line.Status = "Disbursed";
                line.LastDisbursementDate = request.DisbursementDate;
                line.NextDueDate = cycle.CycleType == "Monthly" ? request.DisbursementDate.Date.AddMonths(1) : null;

                result.DisbursedCount++;
                result.TotalAmount += amount.Value;
            }

            if (result.DisbursedCount > 0)
            {
                cycle.Status = "Disbursed";
                cycle.TotalDisbursedAmount = (cycle.TotalDisbursedAmount ?? 0m) + result.TotalAmount;
                await _db.SaveChangesAsync();
            }

            return result;
        }
    }
}
