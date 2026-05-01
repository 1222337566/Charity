using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Payroll;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Reports;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentServices.CharityReports;
using InfrastructureManagmentWebFramework.Models.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

public class CharityReportsController : Controller
{
    private readonly ICharityReportsRepository _reportsRepository;
    private readonly ICharityReportExportService _exportService;
    private readonly AppDbContext _db;

    public CharityReportsController(
        ICharityReportsRepository reportsRepository,
        ICharityReportExportService exportService,
        AppDbContext db)
    {
        _reportsRepository = reportsRepository;
        _exportService = exportService;
        _db = db;
    }

    [HttpGet]
    public IActionResult Index() => View();

    [HttpGet]
    public async Task<IActionResult> BeneficiaryStatuses()
        => View(new BeneficiaryStatusReportVm { Rows = await _reportsRepository.GetBeneficiaryStatusReportAsync() });

    [HttpGet]
    public async Task<IActionResult> PrintBeneficiaryStatuses()
        => View("PrintBeneficiaryStatuses", new BeneficiaryStatusReportVm { Rows = await _reportsRepository.GetBeneficiaryStatusReportAsync() });

    [HttpGet]
    public async Task<IActionResult> BeneficiaryAidDetails(DateTime? fromDate = null, DateTime? toDate = null)
        => View(await BuildBeneficiaryAidDetailsVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> PrintBeneficiaryAidDetails(DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintBeneficiaryAidDetails", await BuildBeneficiaryAidDetailsVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> ExportBeneficiaryStatusesExcel()
    {
        var rows = await _reportsRepository.GetBeneficiaryStatusReportAsync();
        var file = _exportService.ExportBeneficiaryStatusesExcel(rows);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"beneficiary-statuses-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportBeneficiaryStatusesPdf()
    {
        var rows = await _reportsRepository.GetBeneficiaryStatusReportAsync();
        var file = _exportService.ExportBeneficiaryStatusesPdf(rows);
        return File(file, "application/pdf", $"beneficiary-statuses-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> Donations(DateTime? fromDate = null, DateTime? toDate = null)
        => View(new DonationReportVm { FromDate = fromDate, ToDate = toDate, Rows = await _reportsRepository.GetDonationReportAsync(fromDate, toDate) });

    [HttpGet]
    public async Task<IActionResult> PrintDonations(DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintDonations", new DonationReportVm { FromDate = fromDate, ToDate = toDate, Rows = await _reportsRepository.GetDonationReportAsync(fromDate, toDate) });

    [HttpGet]
    public async Task<IActionResult> DonorStatements(Guid? donorId = null, DateTime? fromDate = null, DateTime? toDate = null)
        => View(await BuildDonorStatementsVm(donorId, fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> PrintDonorStatements(Guid? donorId = null, DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintDonorStatements", await BuildDonorStatementsVm(donorId, fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> ExportDonationsExcel(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var rows = await _reportsRepository.GetDonationReportAsync(fromDate, toDate);
        var file = _exportService.ExportDonationsExcel(rows, fromDate, toDate);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"donations-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportDonationsPdf(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var rows = await _reportsRepository.GetDonationReportAsync(fromDate, toDate);
        var file = _exportService.ExportDonationsPdf(rows, fromDate, toDate);
        return File(file, "application/pdf", $"donations-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> Projects()
        => View(new ProjectFinancialReportVm { Rows = await _reportsRepository.GetProjectFinancialReportAsync() });

    [HttpGet]
    public async Task<IActionResult> PrintProjects()
        => View("PrintProjects", new ProjectFinancialReportVm { Rows = await _reportsRepository.GetProjectFinancialReportAsync() });

    [HttpGet]
    public async Task<IActionResult> ProjectDetails(Guid? projectId = null)
        => View(await BuildProjectDetailsVm(projectId));

    [HttpGet]
    public async Task<IActionResult> PrintProjectDetails(Guid? projectId = null)
        => View("PrintProjectDetails", await BuildProjectDetailsVm(projectId));

    [HttpGet]
    public async Task<IActionResult> ExportProjectsExcel()
    {
        var rows = await _reportsRepository.GetProjectFinancialReportAsync();
        var file = _exportService.ExportProjectsExcel(rows);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"projects-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportProjectsPdf()
    {
        var rows = await _reportsRepository.GetProjectFinancialReportAsync();
        var file = _exportService.ExportProjectsPdf(rows);
        return File(file, "application/pdf", $"projects-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> Payroll()
        => View(new PayrollMonthReportVm { Rows = await _reportsRepository.GetPayrollMonthReportAsync() });

    [HttpGet]
    public async Task<IActionResult> PrintPayroll()
        => View("PrintPayroll", new PayrollMonthReportVm { Rows = await _reportsRepository.GetPayrollMonthReportAsync() });

    [HttpGet]
    public async Task<IActionResult> PayrollEmployees(Guid? payrollMonthId = null)
        => View(await BuildPayrollEmployeesVm(payrollMonthId));

    [HttpGet]
    public async Task<IActionResult> PrintPayrollEmployees(Guid? payrollMonthId = null)
        => View("PrintPayrollEmployees", await BuildPayrollEmployeesVm(payrollMonthId));

    [HttpGet]
    public async Task<IActionResult> ExportPayrollExcel()
    {
        var rows = await _reportsRepository.GetPayrollMonthReportAsync();
        var file = _exportService.ExportPayrollExcel(rows);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"payroll-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportPayrollPdf()
    {
        var rows = await _reportsRepository.GetPayrollMonthReportAsync();
        var file = _exportService.ExportPayrollPdf(rows);
        return File(file, "application/pdf", $"payroll-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> StoreMovements(DateTime? fromDate = null, DateTime? toDate = null)
        => View(new StoreMovementReportVm { FromDate = fromDate, ToDate = toDate, Rows = await _reportsRepository.GetStoreMovementReportAsync(fromDate, toDate) });

    [HttpGet]
    public async Task<IActionResult> PrintStoreMovements(DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintStoreMovements", new StoreMovementReportVm { FromDate = fromDate, ToDate = toDate, Rows = await _reportsRepository.GetStoreMovementReportAsync(fromDate, toDate) });

    [HttpGet]
    public async Task<IActionResult> StoreItemMovements(Guid? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null)
        => View(await BuildStoreItemMovementsVm(warehouseId, fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> PrintStoreItemMovements(Guid? warehouseId = null, DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintStoreItemMovements", await BuildStoreItemMovementsVm(warehouseId, fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> ExportStoreMovementsExcel(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var rows = await _reportsRepository.GetStoreMovementReportAsync(fromDate, toDate);
        var file = _exportService.ExportStoreMovementsExcel(rows, fromDate, toDate);
        return File(file, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"store-movements-{DateTime.UtcNow:yyyyMMddHHmmss}.xlsx");
    }

    [HttpGet]
    public async Task<IActionResult> ExportStoreMovementsPdf(DateTime? fromDate = null, DateTime? toDate = null)
    {
        var rows = await _reportsRepository.GetStoreMovementReportAsync(fromDate, toDate);
        var file = _exportService.ExportStoreMovementsPdf(rows, fromDate, toDate);
        return File(file, "application/pdf", $"store-movements-{DateTime.UtcNow:yyyyMMddHHmmss}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> InKindReceiptStatus(DateTime? fromDate = null, DateTime? toDate = null)
        => View(await BuildInKindReceiptStatusVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> PrintInKindReceiptStatus(DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintInKindReceiptStatus", await BuildInKindReceiptStatusVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> InKindBeneficiaryIssues(DateTime? fromDate = null, DateTime? toDate = null)
        => View(await BuildInKindBeneficiaryIssuesVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> PrintInKindBeneficiaryIssues(DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintInKindBeneficiaryIssues", await BuildInKindBeneficiaryIssuesVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> InKindDonationBalances(DateTime? fromDate = null, DateTime? toDate = null)
        => View(await BuildInKindDonationBalancesVm(fromDate, toDate));

    [HttpGet]
    public async Task<IActionResult> PrintInKindDonationBalances(DateTime? fromDate = null, DateTime? toDate = null)
        => View("PrintInKindDonationBalances", await BuildInKindDonationBalancesVm(fromDate, toDate));

    private async Task<BeneficiaryAidDetailReportVm> BuildBeneficiaryAidDetailsVm(DateTime? fromDate, DateTime? toDate)
        => new()
        {
            FromDate = fromDate,
            ToDate = toDate,
            Rows = await _reportsRepository.GetBeneficiaryAidDetailReportAsync(fromDate, toDate)
        };

    private async Task<DonorStatementReportVm> BuildDonorStatementsVm(Guid? donorId, DateTime? fromDate, DateTime? toDate)
    {
        var donors = await _db.Set<Donor>()
            .AsNoTracking()
            .OrderBy(x => x.FullName)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Code + " - " + x.FullName })
            .ToListAsync();

        return new DonorStatementReportVm
        {
            DonorId = donorId,
            FromDate = fromDate,
            ToDate = toDate,
            Donors = donors,
            Rows = await _reportsRepository.GetDonorStatementReportAsync(donorId, fromDate, toDate)
        };
    }

    private async Task<ProjectDetailsReportVm> BuildProjectDetailsVm(Guid? projectId)
    {
        var projects = await _db.Set<CharityProject>()
            .AsNoTracking()
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Code + " - " + x.Name })
            .ToListAsync();

        return new ProjectDetailsReportVm
        {
            ProjectId = projectId,
            Projects = projects,
            Rows = await _reportsRepository.GetProjectDetailsReportAsync(projectId)
        };
    }

    private async Task<PayrollEmployeeDetailReportVm> BuildPayrollEmployeesVm(Guid? payrollMonthId)
    {
        var months = await _db.Set<PayrollMonth>()
            .AsNoTracking()
            .OrderByDescending(x => x.Year)
            .ThenByDescending(x => x.Month)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = $"{x.Month:00}/{x.Year} - {x.Status}" })
            .ToListAsync();

        return new PayrollEmployeeDetailReportVm
        {
            PayrollMonthId = payrollMonthId,
            PayrollMonths = months,
            Rows = await _reportsRepository.GetPayrollEmployeeDetailReportAsync(payrollMonthId)
        };
    }

    private async Task<StoreItemMovementDetailReportVm> BuildStoreItemMovementsVm(Guid? warehouseId, DateTime? fromDate, DateTime? toDate)
    {
        var warehouses = await _db.Set<Warehouse>()
            .AsNoTracking()
            .OrderBy(x => x.WarehouseNameAr)
            .Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.WarehouseCode + " - " + x.WarehouseNameAr })
            .ToListAsync();

        return new StoreItemMovementDetailReportVm
        {
            WarehouseId = warehouseId,
            FromDate = fromDate,
            ToDate = toDate,
            Warehouses = warehouses,
            Rows = await _reportsRepository.GetStoreItemMovementDetailReportAsync(warehouseId, fromDate, toDate)
        };
    }

    private async Task<InKindReceiptStatusReportVm> BuildInKindReceiptStatusVm(DateTime? fromDate, DateTime? toDate)
    {
        var itemsQuery = _db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Donation)!.ThenInclude(x => x!.Donor)
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .AsQueryable();

        if (fromDate.HasValue)
            itemsQuery = itemsQuery.Where(x => x.Donation != null && x.Donation.DonationDate.Date >= fromDate.Value.Date);
        if (toDate.HasValue)
            itemsQuery = itemsQuery.Where(x => x.Donation != null && x.Donation.DonationDate.Date <= toDate.Value.Date);

        var items = await itemsQuery
            .OrderByDescending(x => x.Donation!.DonationDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        var itemIds = items.Select(x => x.Id).ToList();
        var receiptHeaders = await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .Where(x => x.SourceType == "DonationInKind")
            .Select(x => new { x.Id, x.ReceiptNumber, x.ReceiptDate, x.SourceName, x.Notes })
            .ToListAsync();

        var receiptLines = await _db.Set<CharityStoreReceiptLine>()
            .AsNoTracking()
            .Where(x => receiptHeaders.Select(h => h.Id).Contains(x.ReceiptId))
            .Select(x => new { x.ReceiptId, x.ItemId, x.Quantity })
            .ToListAsync();

        var rows = items.Select(item =>
        {
            var relatedReceipts = receiptHeaders
                .Where(r => (!string.IsNullOrWhiteSpace(r.Notes) && r.Notes!.Contains($"DonationInKindItemId:{item.Id}"))
                         || (!string.IsNullOrWhiteSpace(r.SourceName) && item.Donation != null && r.SourceName == item.Donation.DonationNumber))
                .ToList();

            var receivedQuantity = relatedReceipts
                .Join(receiptLines, r => r.Id, l => l.ReceiptId, (r, l) => new { r, l })
                .Where(x => x.l.ItemId == item.ItemId)
                .Sum(x => x.l.Quantity);

            return new InKindReceiptStatusRowVm
            {
                DonationId = item.DonationId,
                DonationItemId = item.Id,
                DonationDate = item.Donation?.DonationDate ?? item.CreatedAtUtc.Date,
                DonationNumber = item.Donation?.DonationNumber ?? string.Empty,
                DonorName = item.Donation?.Donor?.FullName ?? string.Empty,
                ItemName = item.Item?.ItemNameAr ?? string.Empty,
                WarehouseName = item.Warehouse?.WarehouseNameAr,
                DonatedQuantity = item.Quantity,
                ReceivedQuantity = receivedQuantity,
                StoreReceiptCount = relatedReceipts.Count
            };
        }).ToList();

        return new InKindReceiptStatusReportVm
        {
            FromDate = fromDate,
            ToDate = toDate,
            Rows = rows
        };
    }

    private async Task<InKindBeneficiaryIssueReportVm> BuildInKindBeneficiaryIssuesVm(DateTime? fromDate, DateTime? toDate)
    {
        var query = _db.Set<BeneficiaryAidDisbursement>()
            .AsNoTracking()
            .Include(x => x.Beneficiary)
            .Include(x => x.AidType)
            .Include(x => x.FundingLines)!.ThenInclude(x => x.DonationAllocation)!.ThenInclude(x => x!.Donation)
            .Where(x => x.StoreIssueId.HasValue)
            .AsQueryable();

        if (fromDate.HasValue)
            query = query.Where(x => x.DisbursementDate.Date >= fromDate.Value.Date);
        if (toDate.HasValue)
            query = query.Where(x => x.DisbursementDate.Date <= toDate.Value.Date);

        var disbursements = await query.ToListAsync();
        var issueIds = disbursements.Where(x => x.StoreIssueId.HasValue).Select(x => x.StoreIssueId!.Value).Distinct().ToList();

        var issues = await _db.Set<CharityStoreIssue>()
            .AsNoTracking()
            .Include(x => x.Warehouse)
            .Include(x => x.Lines)!.ThenInclude(x => x.Item)
            .Where(x => issueIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id);

        var rows = new List<InKindBeneficiaryIssueRowVm>();
        foreach (var disbursement in disbursements)
        {
            if (!disbursement.StoreIssueId.HasValue || !issues.TryGetValue(disbursement.StoreIssueId.Value, out var issue))
                continue;

            var donationNumbers = new List<string>();
            if (disbursement.DonationId.HasValue)
            {
                var directDonation = await _db.Set<Donation>()
                    .AsNoTracking()
                    .Where(x => x.Id == disbursement.DonationId.Value)
                    .Select(x => x.DonationNumber)
                    .FirstOrDefaultAsync();
                if (!string.IsNullOrWhiteSpace(directDonation))
                    donationNumbers.Add(directDonation);
            }

            donationNumbers.AddRange(disbursement.FundingLines
                .Where(f => f.DonationAllocation?.Donation != null)
                .Select(f => f.DonationAllocation!.Donation!.DonationNumber));

            var donationSummary = donationNumbers
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct()
                .ToList();

            var donationNumber = donationSummary.Count switch
            {
                0 => string.Empty,
                1 => donationSummary[0],
                _ => "متعدد"
            };

            foreach (var line in issue.Lines)
            {
                rows.Add(new InKindBeneficiaryIssueRowVm
                {
                    DisbursementId = disbursement.Id,
                    StoreIssueId = issue.Id,
                    DisbursementDate = disbursement.DisbursementDate,
                    StoreIssueNumber = issue.IssueNumber,
                    DonationNumber = donationNumber,
                    BeneficiaryName = disbursement.Beneficiary?.FullName ?? string.Empty,
                    AidTypeName = disbursement.AidType?.NameAr ?? string.Empty,
                    ItemName = line.Item?.ItemNameAr ?? string.Empty,
                    WarehouseName = issue.Warehouse?.WarehouseNameAr,
                    Quantity = line.Quantity,
                    Notes = disbursement.Notes
                });
            }
        }

        return new InKindBeneficiaryIssueReportVm
        {
            FromDate = fromDate,
            ToDate = toDate,
            Rows = rows
                .OrderByDescending(x => x.DisbursementDate)
                .ThenByDescending(x => x.StoreIssueNumber)
                .ToList()
        };
    }

    private async Task<InKindDonationBalanceReportVm> BuildInKindDonationBalancesVm(DateTime? fromDate, DateTime? toDate)
    {
        var itemsQuery = _db.Set<DonationInKindItem>()
            .AsNoTracking()
            .Include(x => x.Donation)!.ThenInclude(x => x!.Donor)
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .AsQueryable();

        if (fromDate.HasValue)
            itemsQuery = itemsQuery.Where(x => x.Donation != null && x.Donation.DonationDate.Date >= fromDate.Value.Date);
        if (toDate.HasValue)
            itemsQuery = itemsQuery.Where(x => x.Donation != null && x.Donation.DonationDate.Date <= toDate.Value.Date);

        var items = await itemsQuery
            .OrderByDescending(x => x.Donation!.DonationDate)
            .ThenByDescending(x => x.CreatedAtUtc)
            .ToListAsync();

        var itemIds = items.Select(x => x.Id).ToList();
        var allocations = await _db.Set<DonationAllocation>()
            .AsNoTracking()
            .Where(x => x.DonationInKindItemId.HasValue && itemIds.Contains(x.DonationInKindItemId.Value))
            .GroupBy(x => x.DonationInKindItemId!.Value)
            .Select(g => new { DonationInKindItemId = g.Key, AllocatedQuantity = g.Sum(x => x.AllocatedQuantity ?? 0m) })
            .ToListAsync();
        var allocationMap = allocations.ToDictionary(x => x.DonationInKindItemId, x => x.AllocatedQuantity);

        var receiptHeaders = await _db.Set<CharityStoreReceipt>()
            .AsNoTracking()
            .Where(x => x.SourceType == "DonationInKind")
            .Select(x => new { x.Id, x.SourceName, x.Notes })
            .ToListAsync();

        var receiptLines = await _db.Set<CharityStoreReceiptLine>()
            .AsNoTracking()
            .Where(x => receiptHeaders.Select(h => h.Id).Contains(x.ReceiptId))
            .Select(x => new { x.ReceiptId, x.ItemId, x.Quantity })
            .ToListAsync();

        var rows = items.Select(item =>
        {
            var relatedReceipts = receiptHeaders
                .Where(r => (!string.IsNullOrWhiteSpace(r.Notes) && r.Notes!.Contains($"DonationInKindItemId:{item.Id}"))
                         || (!string.IsNullOrWhiteSpace(r.SourceName) && item.Donation != null && r.SourceName == item.Donation.DonationNumber))
                .ToList();

            var receivedQuantity = relatedReceipts
                .Join(receiptLines, r => r.Id, l => l.ReceiptId, (r, l) => new { r, l })
                .Where(x => x.l.ItemId == item.ItemId)
                .Sum(x => x.l.Quantity);

            return new InKindDonationBalanceRowVm
            {
                DonationId = item.DonationId,
                DonationItemId = item.Id,
                DonationDate = item.Donation?.DonationDate ?? item.CreatedAtUtc.Date,
                DonationNumber = item.Donation?.DonationNumber ?? string.Empty,
                DonorName = item.Donation?.Donor?.FullName ?? string.Empty,
                ItemName = item.Item?.ItemNameAr ?? string.Empty,
                WarehouseName = item.Warehouse?.WarehouseNameAr,
                DonatedQuantity = item.Quantity,
                ReceivedQuantity = receivedQuantity,
                AllocatedQuantity = allocationMap.TryGetValue(item.Id, out var allocatedQty) ? allocatedQty : 0m,
                StoreReceiptCount = relatedReceipts.Count
            };
        }).ToList();

        return new InKindDonationBalanceReportVm
        {
            FromDate = fromDate,
            ToDate = toDate,
            Rows = rows
        };
    }

}
