using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Funding;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Beneficiaries
{
    public class AvailableAidDisbursementService : IAvailableAidDisbursementService
    {
        private readonly AppDbContext _db;
        private readonly IAidDisbursementFundingLineService _fundingLineService;

        public AvailableAidDisbursementService(AppDbContext db, IAidDisbursementFundingLineService fundingLineService)
        {
            _db = db;
            _fundingLineService = fundingLineService;
        }

        public async Task<BeneficiaryAidDisbursement?> EnsureAvailableFromApprovedAllocationAsync(Guid allocationId, string? currentUserId, CancellationToken ct = default)
        {
            var allocation = await _db.Set<DonationAllocation>()
                .Include(x => x.Donation)
                .Include(x => x.AidRequest)
                .FirstOrDefaultAsync(x => x.Id == allocationId, ct);

            if (allocation == null
                || allocation.AidRequestId == null
                || allocation.BeneficiaryId == null
                || (allocation.Amount ?? 0m) <= 0m
                || allocation.Donation == null
                || !string.Equals(allocation.Donation.DonationType, "نقدي", StringComparison.OrdinalIgnoreCase)
                || !string.Equals(allocation.ApprovalStatus, DonationAllocationApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
            {
                return null;
            }

            var entity = await _db.Set<BeneficiaryAidDisbursement>()
                .FirstOrDefaultAsync(x => x.SourceType == "DonationAllocation" && x.SourceId == allocation.Id, ct);

            var isNew = entity == null;
            if (entity == null)
            {
                entity = new BeneficiaryAidDisbursement
                {
                    Id = Guid.NewGuid(),
                    SourceType = "DonationAllocation",
                    SourceId = allocation.Id,
                    CreatedByUserId = currentUserId,
                    CreatedAtUtc = DateTime.UtcNow
                };

                await _db.Set<BeneficiaryAidDisbursement>().AddAsync(entity, ct);
            }

            entity.BeneficiaryId = allocation.BeneficiaryId.Value;
            entity.AidRequestId = allocation.AidRequestId;
            entity.AidTypeId = allocation.AidRequest?.AidTypeId ?? entity.AidTypeId;
            entity.DisbursementDate = allocation.ApprovedAtUtc?.Date ?? allocation.AllocatedDate.Date;
            entity.Amount = allocation.Amount;
            entity.DonationId = allocation.DonationId;
            entity.GrantAgreementId = null;
            entity.ApprovalStatus = AidDisbursementApprovalStatusCodes.Approved;
            entity.ApprovedByUserId = allocation.ApprovedByUserId ?? currentUserId;
            entity.ApprovedAtUtc = allocation.ApprovedAtUtc ?? DateTime.UtcNow;
            entity.RejectedByUserId = null;
            entity.RejectedAtUtc = null;
            if (isNew || string.IsNullOrWhiteSpace(entity.ExecutionStatus) || string.Equals(entity.ExecutionStatus, AidDisbursementExecutionStatusCodes.Cancelled, StringComparison.OrdinalIgnoreCase))
            {
                entity.ExecutionStatus = AidDisbursementExecutionStatusCodes.Available;
                entity.ExecutedAmount = 0m;
                entity.ExecutedAtUtc = null;
                entity.ExecutedByUserId = null;
            }

            entity.Notes = $"استحقاق صرف تلقائي من تخصيص التبرع {allocation.Donation?.DonationNumber}";

            await _db.SaveChangesAsync(ct);
            await _fundingLineService.RebuildFundingLinesAsync(entity, currentUserId, ct);
            return entity;
        }

        public async Task<BeneficiaryAidDisbursement?> ExecuteCashAsync(Guid disbursementId, string? currentUserId, CancellationToken ct = default)
        {
            var entity = await _db.Set<BeneficiaryAidDisbursement>()
                .FirstOrDefaultAsync(x => x.Id == disbursementId, ct);

            if (entity == null)
                return null;

            if (!string.Equals(entity.ApprovalStatus, AidDisbursementApprovalStatusCodes.Approved, StringComparison.OrdinalIgnoreCase))
                return entity;

            if (string.Equals(entity.ExecutionStatus, AidDisbursementExecutionStatusCodes.FullyDisbursed, StringComparison.OrdinalIgnoreCase))
                return entity;

            entity.ExecutionStatus = AidDisbursementExecutionStatusCodes.FullyDisbursed;
            entity.ExecutedAmount = entity.Amount;
            entity.ExecutedAtUtc = DateTime.UtcNow;
            entity.ExecutedByUserId = currentUserId;
            await _db.SaveChangesAsync(ct);
            return entity;
        }
    }
}
