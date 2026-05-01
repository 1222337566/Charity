using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Beneficiaries
{
    public sealed class BeneficiaryCommitteeDecisionValidationResult
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public BeneficiaryCommitteeDecision? Decision { get; set; }
    }

    public static class BeneficiaryCommitteeDecisionGuard
    {
        public static async Task<BeneficiaryCommitteeDecisionValidationResult> ValidateAsync(
            AppDbContext db,
            Guid beneficiaryId,
            Guid? aidTypeId = null,
            decimal? amount = null,
            string? operationName = null,
            Guid? specificDecisionId = null)
        {
            var operation = string.IsNullOrWhiteSpace(operationName) ? "تنفيذ العملية" : operationName!.Trim();
            if (beneficiaryId == Guid.Empty)
                return Fail("المستفيد غير محدد.");

            BeneficiaryCommitteeDecision? decision;

            if (specificDecisionId.HasValue && specificDecisionId.Value != Guid.Empty)
            {
                decision = await db.Set<BeneficiaryCommitteeDecision>()
                    .AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == specificDecisionId.Value && x.BeneficiaryId == beneficiaryId);

                if (decision == null)
                    return Fail($"لا يمكن {operation} لأن قرار اللجنة المرتبط غير موجود.");
            }
            else if (aidTypeId.HasValue && aidTypeId.Value != Guid.Empty)
            {
                decision = await db.Set<BeneficiaryCommitteeDecision>()
                    .AsNoTracking()
                    .Where(x => x.BeneficiaryId == beneficiaryId
                                && (x.ApprovedAidTypeId == aidTypeId.Value || x.ApprovedAidTypeId == null))
                    .OrderByDescending(x => x.DecisionDate)
                    .ThenByDescending(x => x.CreatedAtUtc)
                    .FirstOrDefaultAsync();

                if (decision == null)
                    return Fail($"لا يمكن {operation} لأنه لا يوجد قرار لجنة لهذا المستفيد يغطي نوع المساعدة المختار.");
            }
            else
            {
                decision = await db.Set<BeneficiaryCommitteeDecision>()
                    .AsNoTracking()
                    .Where(x => x.BeneficiaryId == beneficiaryId)
                    .OrderByDescending(x => x.DecisionDate)
                    .ThenByDescending(x => x.CreatedAtUtc)
                    .FirstOrDefaultAsync();

                if (decision == null)
                    return Fail($"لا يمكن {operation} لأنه لا يوجد قرار لجنة لهذا المستفيد.");
            }

            if (!decision.ApprovedStatus)
                return Fail($"لا يمكن {operation} لأن آخر قرار لجنة مرتبط بهذه العملية غير معتمد.");

            if (aidTypeId.HasValue && aidTypeId.Value != Guid.Empty && decision.ApprovedAidTypeId.HasValue && decision.ApprovedAidTypeId.Value != aidTypeId.Value)
                return Fail($"لا يمكن {operation} لأن قرار اللجنة المعتمد لا يغطي نوع المساعدة المختار.");

            if (amount.HasValue && amount.Value > 0 && decision.ApprovedAmount.HasValue && amount.Value > decision.ApprovedAmount.Value)
                return Fail($"لا يمكن {operation} لأن القيمة المطلوبة تتجاوز القيمة المعتمدة بقرار اللجنة ({decision.ApprovedAmount.Value:n2}).");

            return new BeneficiaryCommitteeDecisionValidationResult
            {
                IsValid = true,
                Decision = decision,
                Message = "OK"
            };
        }

        public static async Task<BeneficiaryCommitteeDecisionValidationResult> ValidateAidRequestAmountAsync(
            AppDbContext db,
            Guid beneficiaryId,
            Guid aidTypeId,
            decimal? requestedAmount,
            Guid? currentAidRequestId = null)
        {
            var baseValidation = await ValidateAsync(
                db,
                beneficiaryId,
                aidTypeId,
                requestedAmount,
                "إنشاء طلب المساعدة");

            if (!baseValidation.IsValid || baseValidation.Decision == null)
                return baseValidation;

            var decision = baseValidation.Decision;
            if (!decision.ApprovedAmount.HasValue || decision.ApprovedAmount.Value <= 0m)
                return baseValidation;

            var existingRequestedAmount = await db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Where(x => x.BeneficiaryId == beneficiaryId
                            && x.AidTypeId == aidTypeId
                            && (!currentAidRequestId.HasValue || x.Id != currentAidRequestId.Value)
                            && x.Status != "Rejected"
                            && x.Status != "Cancelled"
                            && x.Status != "Canceled")
                .SumAsync(x => x.RequestedAmount ?? 0m);

            var newRequestedAmount = requestedAmount ?? 0m;
            var remainingAmount = Math.Max(0m, decision.ApprovedAmount.Value - existingRequestedAmount);

            if (newRequestedAmount > remainingAmount)
                return Fail($"لا يمكن إنشاء طلب المساعدة لأن إجمالي الطلبات لهذا النوع سيتجاوز قيمة قرار اللجنة. المتاح المتبقي من القرار ({remainingAmount:n2}) من أصل ({decision.ApprovedAmount.Value:n2}).");

            return baseValidation;
        }

        public static async Task<BeneficiaryCommitteeDecisionValidationResult> ValidateAidDisbursementAmountAsync(
            AppDbContext db,
            Guid beneficiaryId,
            Guid aidTypeId,
            decimal? amount,
            Guid? currentDisbursementId = null)
        {
            var baseValidation = await ValidateAsync(
                db,
                beneficiaryId,
                aidTypeId,
                amount,
                "صرف المساعدة");

            if (!baseValidation.IsValid || baseValidation.Decision == null)
                return baseValidation;

            var decision = baseValidation.Decision;
            if (!decision.ApprovedAmount.HasValue || decision.ApprovedAmount.Value <= 0m)
                return baseValidation;

            var existingDisbursedAmount = await db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => x.BeneficiaryId == beneficiaryId
                            && x.AidTypeId == aidTypeId
                            && (!currentDisbursementId.HasValue || x.Id != currentDisbursementId.Value)
                            && x.ApprovalStatus != "Rejected"
                            && x.ExecutionStatus != "Cancelled")
                .SumAsync(x => x.Amount ?? 0m);

            var newAmount = amount ?? 0m;
            var remainingAmount = Math.Max(0m, decision.ApprovedAmount.Value - existingDisbursedAmount);

            if (newAmount > remainingAmount)
                return Fail($"لا يمكن صرف المساعدة لأن إجمالي الصرف لهذا النوع سيتجاوز قيمة قرار اللجنة. المتاح المتبقي من القرار ({remainingAmount:n2}) من أصل ({decision.ApprovedAmount.Value:n2}).");

            return baseValidation;
        }

        public static Task<BeneficiaryCommitteeDecisionValidationResult> ValidateHasApprovedDecisionAsync(
            AppDbContext db,
            Guid beneficiaryId,
            string? operationName = null)
            => ValidateAsync(db, beneficiaryId, null, null, operationName);

        private static BeneficiaryCommitteeDecisionValidationResult Fail(string message)
        {
            return new BeneficiaryCommitteeDecisionValidationResult
            {
                IsValid = false,
                Message = message
            };
        }
    }
}
