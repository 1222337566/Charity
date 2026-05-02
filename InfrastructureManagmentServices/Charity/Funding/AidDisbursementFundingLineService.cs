using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Funding
{
    public class AidDisbursementFundingLineService : IAidDisbursementFundingLineService
    {
        private const decimal Tolerance = 0.0001m;
        private readonly AppDbContext _db;

        public AidDisbursementFundingLineService(AppDbContext db)
        {
            _db = db;
        }

        public async Task<RebuildAidDisbursementFundingLinesResult> RebuildFundingLinesAsync(
            BeneficiaryAidDisbursement disbursement,
            string? currentUserId = null,
            CancellationToken ct = default)
        {
            var result = new RebuildAidDisbursementFundingLinesResult();

            var oldLines = await _db.Set<BeneficiaryAidDisbursementFundingLine>()
                .Where(x => x.DisbursementId == disbursement.Id)
                .ToListAsync(ct);

            if (oldLines.Count > 0)
                _db.Set<BeneficiaryAidDisbursementFundingLine>().RemoveRange(oldLines);

            var requestedAmount = disbursement.Amount ?? 0m;
            if (!disbursement.AidRequestId.HasValue || requestedAmount <= 0m)
            {
                disbursement.DonationId = null;
                await _db.SaveChangesAsync(ct);
                result.IsSuccess = requestedAmount <= 0m;
                result.ErrorMessage = requestedAmount <= 0m ? null : "لا يمكن بناء سطور تمويل للصرف بدون طلب ومبلغ صالحين.";
                return result;
            }

            var candidateAllocations = await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.AidRequestId == disbursement.AidRequestId.Value
                    && x.ApprovalStatus == "Approved"
                    && (!disbursement.DonationId.HasValue || x.DonationId == disbursement.DonationId.Value))
                .Where(x => (x.Amount ?? 0m) > 0m)
                .OrderBy(x => x.AllocatedDate)
                .ThenBy(x => x.CreatedAtUtc)
                .ThenBy(x => x.Id)
                .Select(x => new
                {
                    x.Id,
                    x.DonationId,
                    Amount = x.Amount ?? 0m,
                    x.AllocatedDate,
                    x.CreatedAtUtc
                })
                .ToListAsync(ct);

            if (candidateAllocations.Count == 0)
            {
                disbursement.DonationId = null;
                await _db.SaveChangesAsync(ct);
                result.ErrorMessage = disbursement.DonationId.HasValue
                    ? "لا توجد سطور تخصيص متاحة لهذا التبرع مع طلب المساعدة المحدد."
                    : "لا توجد سطور تخصيص متاحة لطلب المساعدة المحدد.";
                return result;
            }

            var allocationIds = candidateAllocations.Select(x => x.Id).ToList();
            var consumedByAllocation = await _db.Set<BeneficiaryAidDisbursementFundingLine>()
                .AsNoTracking()
                .Where(x => allocationIds.Contains(x.DonationAllocationId) && x.DisbursementId != disbursement.Id)
                .GroupBy(x => x.DonationAllocationId)
                .Select(g => new { DonationAllocationId = g.Key, Amount = g.Sum(x => x.AmountConsumed) })
                .ToDictionaryAsync(x => x.DonationAllocationId, x => x.Amount, ct);

            var remainingToConsume = requestedAmount;
            var newLines = new List<BeneficiaryAidDisbursementFundingLine>();

            foreach (var allocation in candidateAllocations)
            {
                var alreadyConsumed = consumedByAllocation.TryGetValue(allocation.Id, out var consumed) ? consumed : 0m;
                var lineRemaining = Math.Max(0m, allocation.Amount - alreadyConsumed);
                if (lineRemaining <= Tolerance)
                    continue;

                var consumeNow = Math.Min(lineRemaining, remainingToConsume);
                if (consumeNow <= Tolerance)
                    continue;

                newLines.Add(new BeneficiaryAidDisbursementFundingLine
                {
                    Id = Guid.NewGuid(),
                    DisbursementId = disbursement.Id,
                    DonationAllocationId = allocation.Id,
                    AmountConsumed = consumeNow,
                    CreatedAtUtc = DateTime.UtcNow,
                    CreatedByUserId = currentUserId
                });

                remainingToConsume -= consumeNow;
                if (remainingToConsume <= Tolerance)
                    break;
            }

            if (remainingToConsume > Tolerance)
            {
                result.ErrorMessage = disbursement.DonationId.HasValue
                    ? $"الرصيد المتاح داخل سطور التخصيص للتبرع المحدد لا يكفي لتغطية مبلغ الصرف. المتاح أقل من المطلوب بمقدار {remainingToConsume:N2}."
                    : $"الرصيد المتاح داخل سطور التخصيص للطلب لا يكفي لتغطية مبلغ الصرف. العجز {remainingToConsume:N2}.";
                return result;
            }

            if (newLines.Count > 0)
                await _db.Set<BeneficiaryAidDisbursementFundingLine>().AddRangeAsync(newLines, ct);

            var distinctDonationIds = candidateAllocations
                .Where(a => newLines.Any(l => l.DonationAllocationId == a.Id))
                .Select(a => a.DonationId)
                .Distinct()
                .ToList();

            disbursement.DonationId = distinctDonationIds.Count == 1 ? distinctDonationIds[0] : disbursement.DonationId;
            if (distinctDonationIds.Count != 1 && !disbursement.DonationId.HasValue)
            {
                disbursement.DonationId = null;
            }
            else if (distinctDonationIds.Count != 1 && disbursement.DonationId.HasValue && !distinctDonationIds.Contains(disbursement.DonationId.Value))
            {
                disbursement.DonationId = null;
            }

            await _db.SaveChangesAsync(ct);

            result.IsSuccess = true;
            result.LinesCreated = newLines.Count;
            result.TotalConsumedAmount = newLines.Sum(x => x.AmountConsumed);
            result.DonationIdsUsed = distinctDonationIds;
            return result;
        }

        public async Task<Dictionary<Guid, decimal>> GetRemainingAmountByDonationAsync(
            Guid aidRequestId,
            Guid? excludeDisbursementId = null,
            CancellationToken ct = default)
        {
            var allocationLines = await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => x.AidRequestId == aidRequestId && x.ApprovalStatus == "Approved" )
                .Select(x => new { x.Id, x.DonationId, Amount = x.Amount ?? 0m })
                .ToListAsync(ct);

            var allocationIds = allocationLines.Select(x => x.Id).Distinct().ToList();
            if (allocationIds.Count == 0)
                return new Dictionary<Guid, decimal>();

            var consumedByAllocation = await _db.Set<BeneficiaryAidDisbursementFundingLine>()
                .AsNoTracking()
                .Where(x => allocationIds.Contains(x.DonationAllocationId) && (!excludeDisbursementId.HasValue || x.DisbursementId != excludeDisbursementId.Value))
                .GroupBy(x => x.DonationAllocationId)
                .Select(g => new { DonationAllocationId = g.Key, Amount = g.Sum(x => x.AmountConsumed) })
                .ToDictionaryAsync(x => x.DonationAllocationId, x => x.Amount, ct);

            return allocationLines
                .GroupBy(x => x.DonationId)
                .ToDictionary(
                    g => g.Key,
                    g => Math.Max(0m, g.Sum(x => x.Amount - (consumedByAllocation.TryGetValue(x.Id, out var consumed) ? consumed : 0m))));
        }

        public async Task<Dictionary<Guid, string>> BuildDonationTraceSummaryByDisbursementIdsAsync(
            IEnumerable<Guid> disbursementIds,
            CancellationToken ct = default)
        {
            var ids = disbursementIds.Where(x => x != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<Guid, string>();

            var rows = await _db.Set<BeneficiaryAidDisbursementFundingLine>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.DisbursementId))
                .Select(x => new
                {
                    x.DisbursementId,
                    DonationId = x.DonationAllocation != null ? x.DonationAllocation.DonationId : Guid.Empty,
                    DonationNumber = x.DonationAllocation != null && x.DonationAllocation.Donation != null ? x.DonationAllocation.Donation.DonationNumber : null,
                    Amount = x.AmountConsumed
                })
                .ToListAsync(ct);

            return rows
                .GroupBy(x => x.DisbursementId)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var parts = g
                            .Where(x => x.DonationId != Guid.Empty)
                            .GroupBy(x => new { x.DonationId, x.DonationNumber })
                            .Select(x => new
                            {
                                DonationNumber = x.Key.DonationNumber ?? "تبرع",
                                Amount = x.Sum(v => v.Amount)
                            })
                            .OrderByDescending(x => x.Amount)
                            .ToList();

                        if (parts.Count == 0)
                            return string.Empty;

                        if (parts.Count == 1)
                            return $"{parts[0].DonationNumber} ({parts[0].Amount:N2})";

                        var preview = string.Join(" + ", parts.Take(2).Select(x => $"{x.DonationNumber} ({x.Amount:N2})"));
                        return parts.Count > 2 ? $"{preview} + {parts.Count - 2} أخرى" : preview;
                    });
        }
    }
}
