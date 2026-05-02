using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Funding
{
    public class AidRequestFundingService : IAidRequestFundingService
    {
        private const decimal Tolerance = 0.0001m;
        private readonly AppDbContext _db;

        public AidRequestFundingService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<AidRequestFundingSnapshot?> GetSnapshotAsync(
            Guid aidRequestId,
            Guid? excludeAllocationId = null,
            Guid? excludeDisbursementId = null,
            CancellationToken ct = default)
        {
            var result = await GetSnapshotsAsync(new[] { aidRequestId }, excludeAllocationId, excludeDisbursementId, ct);
            return result.TryGetValue(aidRequestId, out var snapshot) ? snapshot : null;
        }

        public async Task<Dictionary<Guid, AidRequestFundingSnapshot>> GetSnapshotsAsync(
            IEnumerable<Guid> aidRequestIds,
            Guid? excludeAllocationId = null,
            Guid? excludeDisbursementId = null,
            CancellationToken ct = default)
        {
            var ids = aidRequestIds
                .Where(x => x != Guid.Empty)
                .Distinct()
                .ToList();

            if (ids.Count == 0)
                return new Dictionary<Guid, AidRequestFundingSnapshot>();

            var requests = await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new
                {
                    x.Id,
                    RequestedAmount = x.RequestedAmount ?? 0m
                })
                .ToListAsync(ct);

            var allocationsQuery = _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => EF.Property<Guid?>(x, "AidRequestId").HasValue
                            && ids.Contains(EF.Property<Guid?>(x, "AidRequestId")!.Value));

            if (excludeAllocationId.HasValue)
                allocationsQuery = allocationsQuery.Where(x => x.Id != excludeAllocationId.Value);

            var fundedByRequest = await allocationsQuery
                .GroupBy(x => EF.Property<Guid?>(x, "AidRequestId")!.Value)
                .Select(g => new
                {
                    AidRequestId = g.Key,
                    Amount = g.Sum(x => x.Amount ?? 0m)
                })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount, ct);

            var disbursementsQuery = _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => x.AidRequestId.HasValue
                    && ids.Contains(x.AidRequestId.Value)
                    && x.ApprovalStatus == AidDisbursementApprovalStatusCodes.Approved
                    && x.ExecutionStatus != AidDisbursementExecutionStatusCodes.Cancelled);

            if (excludeDisbursementId.HasValue)
                disbursementsQuery = disbursementsQuery.Where(x => x.Id != excludeDisbursementId.Value);

            var disbursedByRequest = await disbursementsQuery
                .GroupBy(x => x.AidRequestId!.Value)
                .Select(g => new
                {
                    AidRequestId = g.Key,
                    Amount = g.Sum(x => x.ExecutionStatus == AidDisbursementExecutionStatusCodes.Available
                        ? 0m
                        : (x.ExecutedAmount ?? x.Amount ?? 0m))
                })
                .ToDictionaryAsync(x => x.AidRequestId, x => x.Amount, ct);

            var snapshots = new Dictionary<Guid, AidRequestFundingSnapshot>();
            foreach (var request in requests)
            {
                var funded = fundedByRequest.TryGetValue(request.Id, out var fundedAmount) ? fundedAmount : 0m;
                var disbursed = disbursedByRequest.TryGetValue(request.Id, out var disbursedAmount) ? disbursedAmount : 0m;
                snapshots[request.Id] = BuildSnapshot(request.Id, request.RequestedAmount, funded, disbursed);
            }

            return snapshots;
        }

        private static AidRequestFundingSnapshot BuildSnapshot(Guid aidRequestId, decimal requested, decimal funded, decimal disbursed)
        {
            var remainingToFund = requested > 0m ? Math.Max(0m, requested - funded) : 0m;
            var remainingToDisburse = Math.Max(0m, funded - disbursed);
            var remainingOnRequest = requested > 0m ? Math.Max(0m, requested - disbursed) : 0m;

            var fundingStatusCode = funded <= Tolerance
                ? AidRequestFundingStatusCodes.NotFunded
                : (requested > 0m && funded + Tolerance >= requested
                    ? AidRequestFundingStatusCodes.FullyFunded
                    : AidRequestFundingStatusCodes.PartiallyFunded);

            var disbursementStatusCode = disbursed <= Tolerance
                ? AidRequestFundingStatusCodes.NotDisbursed
                : (requested > 0m && disbursed + Tolerance >= requested
                    ? AidRequestFundingStatusCodes.FullyDisbursed
                    : AidRequestFundingStatusCodes.PartiallyDisbursed);

            return new AidRequestFundingSnapshot
            {
                AidRequestId = aidRequestId,
                RequestedAmount = requested,
                FundedAmount = funded,
                DisbursedAmount = disbursed,
                RemainingToFundAmount = remainingToFund,
                RemainingToDisburseAmount = remainingToDisburse,
                RemainingOnRequestAmount = remainingOnRequest,
                FundingStatusCode = fundingStatusCode,
                DisbursementStatusCode = disbursementStatusCode
            };
        }
    }
}
