using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentServices.Charity.Funding
{
    public class CharityOperationalStatusService : ICharityOperationalStatusService
    {
        private const decimal Tolerance = 0.0001m;
        private readonly AppDbContext _db;
        private readonly IAidRequestFundingService _aidRequestFundingService;

        public CharityOperationalStatusService(AppDbContext db, IAidRequestFundingService aidRequestFundingService)
        {
            _db = db;
            _aidRequestFundingService = aidRequestFundingService;
        }

        public async Task<AidRequestOperationalStatusSnapshot?> GetAidRequestStatusAsync(Guid aidRequestId, CancellationToken ct = default)
        {
            var map = await GetAidRequestStatusesAsync(new[] { aidRequestId }, ct);
            return map.TryGetValue(aidRequestId, out var snapshot) ? snapshot : null;
        }

        public async Task<Dictionary<Guid, AidRequestOperationalStatusSnapshot>> GetAidRequestStatusesAsync(IEnumerable<Guid> aidRequestIds, CancellationToken ct = default)
        {
            var ids = aidRequestIds.Where(x => x != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<Guid, AidRequestOperationalStatusSnapshot>();

            var requests = await _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new { x.Id, x.Status })
                .ToListAsync(ct);

            var fundingSnapshots = await _aidRequestFundingService.GetSnapshotsAsync(ids, ct: ct);
            var result = new Dictionary<Guid, AidRequestOperationalStatusSnapshot>();

            foreach (var request in requests)
            {
                fundingSnapshots.TryGetValue(request.Id, out var funding);
                funding ??= new AidRequestFundingSnapshot { AidRequestId = request.Id };

                var operationalCode = CharityOperationalStatusCodes.Open;
                var operationalName = "مفتوح";
                var isClosed = false;

                if (string.Equals(request.Status, "Rejected", StringComparison.OrdinalIgnoreCase))
                {
                    operationalCode = CharityOperationalStatusCodes.Rejected;
                    operationalName = "مرفوض";
                }
                else if (string.Equals(request.Status, "Pending", StringComparison.OrdinalIgnoreCase))
                {
                    operationalCode = CharityOperationalStatusCodes.Pending;
                    operationalName = "قيد الإجراء";
                }
                else if (funding.DisbursementStatusCode == AidRequestFundingStatusCodes.FullyDisbursed)
                {
                    operationalCode = CharityOperationalStatusCodes.Closed;
                    operationalName = "مغلق";
                    isClosed = true;
                }

                result[request.Id] = new AidRequestOperationalStatusSnapshot
                {
                    AidRequestId = request.Id,
                    WorkflowStatus = request.Status,
                    OperationalStatusCode = operationalCode,
                    OperationalStatusName = operationalName,
                    IsClosed = isClosed,
                    RequestedAmount = funding.RequestedAmount,
                    FundedAmount = funding.FundedAmount,
                    DisbursedAmount = funding.DisbursedAmount,
                    RemainingToFundAmount = funding.RemainingToFundAmount,
                    RemainingToDisburseAmount = funding.RemainingToDisburseAmount,
                    FundingStatusCode = funding.FundingStatusCode,
                    FundingStatusName = funding.FundingStatusName,
                    DisbursementStatusCode = funding.DisbursementStatusCode,
                    DisbursementStatusName = funding.DisbursementStatusName
                };
            }

            return result;
        }

        public async Task<DonationOperationalStatusSnapshot?> GetDonationStatusAsync(Guid donationId, CancellationToken ct = default)
        {
            var map = await GetDonationStatusesAsync(new[] { donationId }, ct);
            return map.TryGetValue(donationId, out var snapshot) ? snapshot : null;
        }

        public async Task<Dictionary<Guid, DonationOperationalStatusSnapshot>> GetDonationStatusesAsync(IEnumerable<Guid> donationIds, CancellationToken ct = default)
        {
            var ids = donationIds.Where(x => x != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
                return new Dictionary<Guid, DonationOperationalStatusSnapshot>();

            var donations = await _db.Set<Donation>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.Id))
                .Select(x => new { x.Id, x.DonationNumber, x.DonationType, Amount = x.Amount ?? 0m })
                .ToListAsync(ct);

            var allocatedAmounts = await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.DonationId))
                .GroupBy(x => x.DonationId)
                .Select(g => new { DonationId = g.Key, Amount = g.Sum(x => x.Amount ?? 0m), Qty = g.Sum(x => x.AllocatedQuantity ?? 0m) })
                .ToListAsync(ct);
            var allocatedAmountMap = allocatedAmounts.ToDictionary(x => x.DonationId, x => x.Amount);
            var allocatedQtyMap = allocatedAmounts.ToDictionary(x => x.DonationId, x => x.Qty);

            var itemRows = await _db.Set<DonationInKindItem>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.DonationId))
                .Select(x => new { x.Id, x.DonationId, x.Quantity })
                .ToListAsync(ct);
            var donationNumbers = donations.Select(x => x.DonationNumber).Distinct().ToList();

            var receiptRows = await _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.SourceType == "DonationInKind" && x.SourceName != null && donationNumbers.Contains(x.SourceName))
                .ToListAsync(ct);
            var receivedQtyByNumber = receiptRows
                .GroupBy(x => x.SourceName!)
                .ToDictionary(g => g.Key, g => g.SelectMany(x => x.Lines).Sum(l => l.Quantity));

            var allocations = await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => ids.Contains(x.DonationId))
                .Select(x => new { x.Id, x.DonationId, Qty = x.AllocatedQuantity ?? 0m })
                .ToListAsync(ct);
            var allocationDonationMap = allocations.ToDictionary(x => x.Id, x => x.DonationId);

            var issueRows = await _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Lines)
                .Where(x => x.Notes != null && x.Notes.Contains("DonationAllocationId:"))
                .ToListAsync(ct);

            var issuedQtyByDonation = new Dictionary<Guid, decimal>();
            foreach (var issue in issueRows)
            {
                var allocationId = TryExtractAllocationId(issue.Notes);
                if (!allocationId.HasValue || !allocationDonationMap.TryGetValue(allocationId.Value, out var donationId))
                    continue;

                issuedQtyByDonation.TryGetValue(donationId, out var current);
                issuedQtyByDonation[donationId] = current + issue.Lines.Sum(x => x.Quantity);
            }

            var result = new Dictionary<Guid, DonationOperationalStatusSnapshot>();
            foreach (var donation in donations)
            {
                var snapshot = new DonationOperationalStatusSnapshot
                {
                    DonationId = donation.Id,
                    DonationType = donation.DonationType,
                    DonationAmount = donation.Amount,
                    AllocatedAmount = allocatedAmountMap.TryGetValue(donation.Id, out var allocatedAmount) ? allocatedAmount : 0m,
                    AllocatedQuantity = allocatedQtyMap.TryGetValue(donation.Id, out var allocatedQty) ? allocatedQty : 0m,
                    TotalQuantity = itemRows.Where(x => x.DonationId == donation.Id).Sum(x => x.Quantity),
                    ReceivedQuantity = receivedQtyByNumber.TryGetValue(donation.DonationNumber, out var receivedQty) ? receivedQty : 0m,
                    IssuedQuantity = issuedQtyByDonation.TryGetValue(donation.Id, out var issuedQty) ? issuedQty : 0m
                };

                snapshot.RemainingAmount = Math.Max(0m, snapshot.DonationAmount - snapshot.AllocatedAmount);
                snapshot.RemainingQuantity = Math.Max(0m, snapshot.TotalQuantity - snapshot.IssuedQuantity);

                if (string.Equals(donation.DonationType, "نقدي", StringComparison.OrdinalIgnoreCase))
                {
                    if (snapshot.AllocatedAmount <= Tolerance)
                    {
                        snapshot.AllocationStatusCode = CharityOperationalStatusCodes.Unallocated;
                        snapshot.AllocationStatusName = "غير مخصص";
                    }
                    else if (snapshot.DonationAmount > 0m && snapshot.AllocatedAmount + Tolerance >= snapshot.DonationAmount)
                    {
                        snapshot.AllocationStatusCode = CharityOperationalStatusCodes.FullyAllocated;
                        snapshot.AllocationStatusName = "مخصص بالكامل";
                    }
                    else
                    {
                        snapshot.AllocationStatusCode = CharityOperationalStatusCodes.PartiallyAllocated;
                        snapshot.AllocationStatusName = "مخصص جزئيًا";
                    }

                    snapshot.OperationalStatusCode = snapshot.AllocationStatusCode == CharityOperationalStatusCodes.FullyAllocated
                        ? CharityOperationalStatusCodes.Closed
                        : CharityOperationalStatusCodes.Open;
                    snapshot.OperationalStatusName = snapshot.AllocationStatusCode == CharityOperationalStatusCodes.FullyAllocated
                        ? "مغلق"
                        : "مفتوح";
                    snapshot.IsClosed = snapshot.AllocationStatusCode == CharityOperationalStatusCodes.FullyAllocated;
                }
                else
                {
                    if (snapshot.ReceivedQuantity <= Tolerance)
                    {
                        snapshot.StoreStatusCode = CharityOperationalStatusCodes.NotReceivedToStore;
                        snapshot.StoreStatusName = "لم يدخل المخزن";
                    }
                    else if (snapshot.TotalQuantity > 0m && snapshot.ReceivedQuantity + Tolerance >= snapshot.TotalQuantity)
                    {
                        snapshot.StoreStatusCode = CharityOperationalStatusCodes.ReceivedToStore;
                        snapshot.StoreStatusName = "دخل المخزن";
                    }
                    else
                    {
                        snapshot.StoreStatusCode = CharityOperationalStatusCodes.PartiallyReceivedToStore;
                        snapshot.StoreStatusName = "دخل جزئيًا";
                    }

                    if (snapshot.IssuedQuantity <= Tolerance)
                    {
                        snapshot.IssueStatusCode = CharityOperationalStatusCodes.NotIssued;
                        snapshot.IssueStatusName = "لم يُصرف";
                    }
                    else if (snapshot.TotalQuantity > 0m && snapshot.IssuedQuantity + Tolerance >= snapshot.TotalQuantity)
                    {
                        snapshot.IssueStatusCode = CharityOperationalStatusCodes.FullyIssued;
                        snapshot.IssueStatusName = "صُرف بالكامل";
                    }
                    else
                    {
                        snapshot.IssueStatusCode = CharityOperationalStatusCodes.PartiallyIssued;
                        snapshot.IssueStatusName = "صُرف جزئيًا";
                    }

                    snapshot.OperationalStatusCode = snapshot.IssueStatusCode == CharityOperationalStatusCodes.FullyIssued
                        ? CharityOperationalStatusCodes.Closed
                        : CharityOperationalStatusCodes.Open;
                    snapshot.OperationalStatusName = snapshot.IssueStatusCode == CharityOperationalStatusCodes.FullyIssued
                        ? "مغلق"
                        : "مفتوح";
                    snapshot.IsClosed = snapshot.IssueStatusCode == CharityOperationalStatusCodes.FullyIssued;
                }

                result[donation.Id] = snapshot;
            }

            return result;
        }

        private static Guid? TryExtractAllocationId(string? notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return null;

            const string marker = "DonationAllocationId:";
            var index = notes.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
            if (index < 0)
                return null;

            var value = notes[(index + marker.Length)..].Trim();
            var separators = new[] { '|', ' ', ' ', ' ', ';', ',' };
            var token = value.Split(separators, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
            return Guid.TryParse(token, out var id) ? id : null;
        }
    }
}
