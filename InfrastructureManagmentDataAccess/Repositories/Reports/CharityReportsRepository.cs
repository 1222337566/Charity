using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastrfuctureManagmentCore.Queries.Reports;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Reports
{
    public class CharityReportsRepository : ICharityReportsRepository
    {
        private readonly AppDbContext _db;
        public CharityReportsRepository(AppDbContext db) => _db = db;

        public async Task<List<BeneficiaryStatusReportRowDto>> GetBeneficiaryStatusReportAsync()
        {
            var statuses = await _db.Set<BeneficiaryStatusLookup>()
                .AsNoTracking()
                .OrderBy(x => x.DisplayOrder)
                .ThenBy(x => x.NameAr)
                .ToListAsync();

            var beneficiaries = await _db.Set<Beneficiary>().AsNoTracking().ToListAsync();
            var requests = await _db.Set<BeneficiaryAidRequest>().AsNoTracking().ToListAsync();
            var disbursements = await _db.Set<BeneficiaryAidDisbursement>().AsNoTracking().ToListAsync();

            return statuses.Select(s =>
            {
                var beneficiaryIds = beneficiaries.Where(b => b.StatusId == s.Id).Select(b => b.Id).ToHashSet();
                var relatedRequests = requests.Where(r => beneficiaryIds.Contains(r.BeneficiaryId)).ToList();
                var relatedDisbursements = disbursements.Where(d => beneficiaryIds.Contains(d.BeneficiaryId)).ToList();

                return new BeneficiaryStatusReportRowDto
                {
                    StatusName = s.NameAr,
                    BeneficiariesCount = beneficiaryIds.Count,
                    AidRequestsCount = relatedRequests.Count,
                    DisbursementsCount = relatedDisbursements.Count,
                    TotalDisbursedAmount = relatedDisbursements.Sum(x => x.Amount ?? 0m)
                };
            }).ToList();
        }

        public async Task<List<DonationReportRowDto>> GetDonationReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = _db.Set<Donation>()
                .AsNoTracking()
                .Include(x => x.Donor)
                .Include(x => x.PaymentMethod)
                .Include(x => x.FinancialAccount)
                .AsQueryable();

            if (fromDate.HasValue) query = query.Where(x => x.DonationDate >= fromDate.Value.Date);
            if (toDate.HasValue) query = query.Where(x => x.DonationDate <= toDate.Value.Date);

            return await query
                .OrderByDescending(x => x.DonationDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new DonationReportRowDto
                {
                    DonationId = x.Id,
                    DonationDate = x.DonationDate,
                    DonationNumber = x.DonationNumber,
                    DonorName = x.Donor != null ? x.Donor.FullName : string.Empty,
                    DonationType = x.DonationType,
                    Amount = x.Amount ?? 0m,
                    PaymentMethodName = x.PaymentMethod != null ? x.PaymentMethod.MethodNameAr : string.Empty,
                    FinancialAccountName = x.FinancialAccount != null ? x.FinancialAccount.AccountNameAr : string.Empty,
                    IsRestricted = x.IsRestricted,
                    CampaignName = x.CampaignName
                })
                .ToListAsync();
        }

        public async Task<List<ProjectFinancialReportRowDto>> GetProjectFinancialReportAsync()
        {
            return await _db.Set<CharityProject>()
                .AsNoTracking()
                .Select(x => new ProjectFinancialReportRowDto
                {
                    ProjectId = x.Id,
                    ProjectCode = x.Code,
                    ProjectName = x.Name,
                    Status = x.Status,
                    Budget = x.Budget,
                    PlannedBudgetLines = x.BudgetLines.Sum(b => b.PlannedAmount),
                    ActualBudgetLines = x.BudgetLines.Sum(b => b.ActualAmount),
                    AllocatedGrants = x.Grants.Sum(g => g.AllocatedAmount),
                    BeneficiariesCount = x.Beneficiaries.Count,
                    ActivitiesCount = x.Activities.Count
                })
                .OrderBy(x => x.ProjectName)
                .ToListAsync();
        }

        public async Task<List<PayrollMonthReportRowDto>> GetPayrollMonthReportAsync()
        {
            return await _db.Set<PayrollMonth>()
                .AsNoTracking()
                .Select(x => new PayrollMonthReportRowDto
                {
                    PayrollMonthId = x.Id,
                    Year = x.Year,
                    Month = x.Month,
                    Status = x.Status,
                    EmployeesCount = x.Employees.Count,
                    TotalBasic = x.Employees.Sum(e => e.BasicSalary),
                    TotalAdditions = x.Employees.Sum(e => e.Additions),
                    TotalDeductions = x.Employees.Sum(e => e.TotalDeductions),
                    TotalNet = x.Employees.Sum(e => e.NetAmount)
                })
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Month)
                .ToListAsync();
        }

        public async Task<List<StoreMovementReportRowDto>> GetStoreMovementReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var receiptsQuery = _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .AsQueryable();

            var issuesQuery = _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .AsQueryable();

            if (fromDate.HasValue)
            {
                receiptsQuery = receiptsQuery.Where(x => x.ReceiptDate >= fromDate.Value.Date);
                issuesQuery = issuesQuery.Where(x => x.IssueDate >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                receiptsQuery = receiptsQuery.Where(x => x.ReceiptDate <= toDate.Value.Date);
                issuesQuery = issuesQuery.Where(x => x.IssueDate <= toDate.Value.Date);
            }

            var receipts = await receiptsQuery.ToListAsync();
            var issues = await issuesQuery.ToListAsync();

            var warehouseKeys = receipts.Select(x => new { x.WarehouseId, Name = x.Warehouse != null ? x.Warehouse.WarehouseNameAr : string.Empty })
                .Concat(issues.Select(x => new { x.WarehouseId, Name = x.Warehouse != null ? x.Warehouse.WarehouseNameAr : string.Empty }))
                .Distinct()
                .ToList();

            return warehouseKeys
                .Select(k => new StoreMovementReportRowDto
                {
                    WarehouseName = k.Name,
                    ReceiptsCount = receipts.Count(x => x.WarehouseId == k.WarehouseId),
                    ReceiptQuantity = receipts.Where(x => x.WarehouseId == k.WarehouseId).SelectMany(x => x.Lines).Sum(l => l.Quantity),
                    IssuesCount = issues.Count(x => x.WarehouseId == k.WarehouseId),
                    IssueQuantity = issues.Where(x => x.WarehouseId == k.WarehouseId).SelectMany(x => x.Lines).Sum(l => l.Quantity)
                })
                .OrderByDescending(x => x.ReceiptQuantity + x.IssueQuantity)
                .ThenBy(x => x.WarehouseName)
                .ToList();
        }

        public async Task<List<BeneficiaryAidDetailReportRowDto>> GetBeneficiaryAidDetailReportAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var requestsQuery = _db.Set<BeneficiaryAidRequest>()
                .AsNoTracking()
                .Include(x => x.Beneficiary)!
                    .ThenInclude(x => x!.Status)
                .Include(x => x.AidType)
                .AsQueryable();

            if (fromDate.HasValue) requestsQuery = requestsQuery.Where(x => x.RequestDate >= fromDate.Value.Date);
            if (toDate.HasValue) requestsQuery = requestsQuery.Where(x => x.RequestDate <= toDate.Value.Date);

            var requests = await requestsQuery
                .OrderByDescending(x => x.RequestDate)
                .ThenBy(x => x.Beneficiary!.FullName)
                .ToListAsync();

            var requestIds = requests.Select(x => x.Id).ToHashSet();
            var beneficiaryIds = requests.Select(x => x.BeneficiaryId).ToHashSet();

            var disbursements = await _db.Set<BeneficiaryAidDisbursement>()
                .AsNoTracking()
                .Where(x => requestIds.Contains(x.AidRequestId ?? Guid.Empty) || beneficiaryIds.Contains(x.BeneficiaryId))
                .ToListAsync();

            return requests.Select(x =>
            {
                var related = disbursements.Where(d => d.AidRequestId == x.Id || (d.AidRequestId == null && d.BeneficiaryId == x.BeneficiaryId)).ToList();
                return new BeneficiaryAidDetailReportRowDto
                {
                    BeneficiaryId = x.BeneficiaryId,
                    BeneficiaryCode = x.Beneficiary?.Code ?? string.Empty,
                    BeneficiaryName = x.Beneficiary?.FullName ?? string.Empty,
                    BeneficiaryStatus = x.Beneficiary?.Status?.NameAr ?? string.Empty,
                    RequestDate = x.RequestDate,
                    AidTypeName = x.AidType?.NameAr ?? string.Empty,
                    RequestedAmount = x.RequestedAmount ?? 0m,
                    RequestStatus = x.Status,
                    DisbursedAmount = related.Sum(d => d.Amount ?? 0m),
                    LastDisbursementDate = related.OrderByDescending(d => d.DisbursementDate).Select(d => (DateTime?)d.DisbursementDate).FirstOrDefault()
                };
            }).ToList();
        }

        public async Task<List<DonorStatementReportRowDto>> GetDonorStatementReportAsync(Guid? donorId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var donationsQuery = _db.Set<Donation>()
                .AsNoTracking()
                .Include(x => x.Donor)
                .AsQueryable();

            if (donorId.HasValue) donationsQuery = donationsQuery.Where(x => x.DonorId == donorId.Value);
            if (fromDate.HasValue) donationsQuery = donationsQuery.Where(x => x.DonationDate >= fromDate.Value.Date);
            if (toDate.HasValue) donationsQuery = donationsQuery.Where(x => x.DonationDate <= toDate.Value.Date);

            var donations = await donationsQuery
                .OrderByDescending(x => x.DonationDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

            var donationIds = donations.Select(x => x.Id).ToHashSet();
            var allocations = await _db.Set<DonationAllocation>()
                .AsNoTracking()
                .Where(x => donationIds.Contains(x.DonationId))
                .ToListAsync();

            return donations.Select(x =>
            {
                var allocatedAmount = allocations.Where(a => a.DonationId == x.Id).Sum(a => a.Amount ?? 0m);
                var donationAmount = x.Amount ?? 0m;
                return new DonorStatementReportRowDto
                {
                    DonorId = x.DonorId,
                    DonorName = x.Donor?.FullName ?? string.Empty,
                    DonationDate = x.DonationDate,
                    DonationNumber = x.DonationNumber,
                    DonationType = x.DonationType,
                    DonationAmount = donationAmount,
                    AllocatedAmount = allocatedAmount,
                    RemainingAmount = donationAmount - allocatedAmount,
                    CampaignName = x.CampaignName,
                    IsRestricted = x.IsRestricted
                };
            }).ToList();
        }

        public async Task<List<ProjectDetailsReportRowDto>> GetProjectDetailsReportAsync(Guid? projectId = null)
        {
            var query = _db.Set<CharityProject>().AsNoTracking().AsQueryable();
            if (projectId.HasValue) query = query.Where(x => x.Id == projectId.Value);

            return await query
                .Select(x => new ProjectDetailsReportRowDto
                {
                    ProjectId = x.Id,
                    ProjectCode = x.Code,
                    ProjectName = x.Name,
                    Status = x.Status,
                    StartDate = x.StartDate,
                    EndDate = x.EndDate,
                    Location = x.Location,
                    Budget = x.Budget,
                    PlannedBudgetLines = x.BudgetLines.Sum(b => b.PlannedAmount),
                    ActualBudgetLines = x.BudgetLines.Sum(b => b.ActualAmount),
                    AllocatedGrants = x.Grants.Sum(g => g.AllocatedAmount),
                    BeneficiariesCount = x.Beneficiaries.Count,
                    ActivitiesCount = x.Activities.Count,
                    ExecutionPercentage = x.Budget > 0 ? Math.Round((x.BudgetLines.Sum(b => b.ActualAmount) / x.Budget) * 100m, 2) : 0m
                })
                .OrderBy(x => x.ProjectName)
                .ToListAsync();
        }

        public async Task<List<PayrollEmployeeDetailReportRowDto>> GetPayrollEmployeeDetailReportAsync(Guid? payrollMonthId = null)
        {
            var query = _db.Set<PayrollEmployee>()
                .AsNoTracking()
                .Include(x => x.Employee)!
                    .ThenInclude(x => x!.Department)
                .Include(x => x.PayrollMonth)
                .AsQueryable();

            if (payrollMonthId.HasValue) query = query.Where(x => x.PayrollMonthId == payrollMonthId.Value);

            return await query
                .OrderBy(x => x.PayrollMonth!.Year)
                .ThenBy(x => x.PayrollMonth!.Month)
                .ThenBy(x => x.Employee!.FullName)
                .Select(x => new PayrollEmployeeDetailReportRowDto
                {
                    PayrollMonthId = x.PayrollMonthId,
                    EmployeeId = x.EmployeeId,
                    EmployeeCode = x.Employee != null ? x.Employee.Code : string.Empty,
                    EmployeeName = x.Employee != null ? x.Employee.FullName : string.Empty,
                    DepartmentName = x.Employee != null && x.Employee.Department != null ? x.Employee.Department.Name : string.Empty,
                    BasicSalary = x.BasicSalary,
                    Additions = x.Additions,
                    AttendanceDeduction = x.AttendanceDeduction,
                    OtherDeductions = x.OtherDeductions,
                    TotalDeductions = x.TotalDeductions,
                    NetAmount = x.NetAmount
                })
                .ToListAsync();
        }

        public async Task<List<StoreItemMovementDetailReportRowDto>> GetStoreItemMovementDetailReportAsync(Guid? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var receiptsQuery = _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)!
                    .ThenInclude(x => x.Item)
                .AsQueryable();

            var issuesQuery = _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)!
                    .ThenInclude(x => x.Item)
                .AsQueryable();

            if (warehouseId.HasValue)
            {
                receiptsQuery = receiptsQuery.Where(x => x.WarehouseId == warehouseId.Value);
                issuesQuery = issuesQuery.Where(x => x.WarehouseId == warehouseId.Value);
            }
            if (fromDate.HasValue)
            {
                receiptsQuery = receiptsQuery.Where(x => x.ReceiptDate >= fromDate.Value.Date);
                issuesQuery = issuesQuery.Where(x => x.IssueDate >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                receiptsQuery = receiptsQuery.Where(x => x.ReceiptDate <= toDate.Value.Date);
                issuesQuery = issuesQuery.Where(x => x.IssueDate <= toDate.Value.Date);
            }

            var receipts = await receiptsQuery.ToListAsync();
            var issues = await issuesQuery.ToListAsync();

            var receiptRows = receipts.SelectMany(r => r.Lines.Select(l => new
            {
                r.WarehouseId,
                WarehouseName = r.Warehouse != null ? r.Warehouse.WarehouseNameAr : string.Empty,
                l.ItemId,
                ItemCode = l.Item != null ? l.Item.ItemCode : string.Empty,
                ItemName = l.Item != null ? l.Item.ItemNameAr : string.Empty,
                Received = l.Quantity,
                Issued = 0m
            }));

            var issueRows = issues.SelectMany(r => r.Lines.Select(l => new
            {
                r.WarehouseId,
                WarehouseName = r.Warehouse != null ? r.Warehouse.WarehouseNameAr : string.Empty,
                l.ItemId,
                ItemCode = l.Item != null ? l.Item.ItemCode : string.Empty,
                ItemName = l.Item != null ? l.Item.ItemNameAr : string.Empty,
                Received = 0m,
                Issued = l.Quantity
            }));

            return receiptRows
                .Concat(issueRows)
                .GroupBy(x => new { x.WarehouseId, x.WarehouseName, x.ItemId, x.ItemCode, x.ItemName })
                .Select(g => new StoreItemMovementDetailReportRowDto
                {
                    WarehouseId = g.Key.WarehouseId,
                    WarehouseName = g.Key.WarehouseName,
                    ItemId = g.Key.ItemId,
                    ItemCode = g.Key.ItemCode,
                    ItemName = g.Key.ItemName,
                    ReceivedQuantity = g.Sum(x => x.Received),
                    IssuedQuantity = g.Sum(x => x.Issued)
                })
                .OrderByDescending(x => x.ReceivedQuantity + x.IssuedQuantity)
                .ThenBy(x => x.WarehouseName)
                .ThenBy(x => x.ItemName)
                .ToList();
        }
    }
}
