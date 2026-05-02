using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Funding;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Funding;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class GrantorFinancialReportsController : Controller
    {
        private const string SourceDirectExpense = "DirectExpense";
        private const string SourcePhaseExpense = "PhaseExpense";
        private const string SourceStoreIssue = "StoreIssue";

        private readonly AppDbContext _db;

        public GrantorFinancialReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            Guid? grantorId,
            Guid? projectId,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken ct)
        {
            var vm = new GrantorFinancialReportPageVm
            {
                GrantorId = grantorId,
                ProjectId = projectId,
                FromDate = fromDate,
                ToDate = toDate
            };

            await FillLookupsAsync(vm, ct);

            var agreementsQuery = _db.Set<ProjectFundingAgreement>()
                .AsNoTracking()
                .Include(x => x.Grantor)
                .Include(x => x.Project)
                .Include(x => x.Installments)
                .AsQueryable();

            if (grantorId.HasValue)
                agreementsQuery = agreementsQuery.Where(x => x.GrantorId == grantorId.Value);

            if (projectId.HasValue)
                agreementsQuery = agreementsQuery.Where(x => x.ProjectId == projectId.Value);

            var agreements = await agreementsQuery.ToListAsync(ct);

            var projectIds = agreements
                .Select(x => x.ProjectId)
                .Distinct()
                .ToList();

            vm.Agreements = agreements.Select(x => new GrantorFinancialAgreementRowVm
            {
                AgreementId = x.Id,
                AgreementNumber = x.AgreementNumber,
                GrantorName = x.Grantor?.NameAr ?? string.Empty,
                ProjectName = x.Project?.Name ?? string.Empty,
                FundingAmount = x.FundingAmount,
                ReceivedAmount = x.Installments.Where(i => i.Status == "Received").Sum(i => i.Amount),
                StartDateUtc = x.StartDateUtc,
                EndDateUtc = x.EndDateUtc,
                Status = x.Status,
                ContactPerson = x.ContactPerson,
                ContactEmail = x.ContactEmail
            }).ToList();

            var statementRows = new List<GrantorFinancialStatementRowVm>();

            statementRows.AddRange(await LoadDirectExpenseRowsAsync(projectIds, agreements, fromDate, toDate, ct));
            statementRows.AddRange(await LoadPhaseExpenseRowsAsync(projectIds, agreements, fromDate, toDate, ct));
            statementRows.AddRange(await LoadStoreIssueRowsAsync(projectIds, agreements, fromDate, toDate, ct));

            vm.StatementRows = statementRows
                .OrderByDescending(x => x.DocumentDateUtc)
                .ThenBy(x => x.ProjectName)
                .ToList();

            vm.BudgetRows = await BuildBudgetRowsAsync(projectIds, vm.StatementRows, ct);

            return View(vm);
        }

        private async Task<List<GrantorFinancialStatementRowVm>> LoadDirectExpenseRowsAsync(
            List<Guid> projectIds,
            List<ProjectFundingAgreement> agreements,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken ct)
        {
            var query = _db.Set<ProjectExpenseLink>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Expense)
                .Include(x => x.ProjectBudgetLine)
                .Include(x => x.CostCenter)
                .Where(x => projectIds.Contains(x.ProjectId) && x.Expense != null && x.Project != null);

            if (fromDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc <= toDate.Value);

            var items = await query.ToListAsync(ct);

            return items.Select(x =>
            {
                var agreement = agreements.FirstOrDefault(a => a.ProjectId == x.ProjectId);

                return new GrantorFinancialStatementRowVm
                {
                    ProjectId = x.ProjectId,
                    ProjectName = x.Project?.Name ?? string.Empty,
                    GrantorName = agreement?.Grantor?.NameAr ?? string.Empty,
                    AgreementNumber = agreement?.AgreementNumber ?? string.Empty,
                    BudgetLineId = x.ProjectBudgetLineId,
                    BudgetLineName = x.ProjectBudgetLine?.LineName,
                    PhaseName = null,
                    CostCenterName = x.CostCenter?.NameAr,
                    DocumentDateUtc = x.Expense?.ExpenseDateUtc ?? x.CreatedAtUtc,
                    DocumentNumber = x.Expense?.ExpenseNumber ?? string.Empty,
                    SourceKind = SourceDirectExpense,
                    SourceKindAr = "مصروف مباشر",
                    ExpenseItem = x.ProjectBudgetLine?.LineName ?? x.Expense?.Description ?? "مصروف",
                    Amount = x.Expense?.Amount ?? 0m,
                    IncludeInActualCost = true,
                    Notes = x.Notes
                };
            }).ToList();
        }

        private async Task<List<GrantorFinancialStatementRowVm>> LoadPhaseExpenseRowsAsync(
            List<Guid> projectIds,
            List<ProjectFundingAgreement> agreements,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken ct)
        {
            var query = _db.Set<ProjectPhaseExpenseLink>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.ProjectPhase)
                .Include(x => x.Expense)
                .Include(x => x.ProjectBudgetLine)
                .Include(x => x.CostCenter)
                .Where(x => projectIds.Contains(x.ProjectId) && x.Expense != null && x.Project != null);

            if (fromDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc <= toDate.Value);

            var items = await query.ToListAsync(ct);

            return items.Select(x =>
            {
                var agreement = agreements.FirstOrDefault(a => a.ProjectId == x.ProjectId);

                return new GrantorFinancialStatementRowVm
                {
                    ProjectId = x.ProjectId,
                    ProjectName = x.Project?.Name ?? string.Empty,
                    GrantorName = agreement?.Grantor?.NameAr ?? string.Empty,
                    AgreementNumber = agreement?.AgreementNumber ?? string.Empty,
                    BudgetLineId = x.ProjectBudgetLineId,
                    BudgetLineName = x.ProjectBudgetLine?.LineName,
                    PhaseName = x.ProjectPhase?.Name,
                    CostCenterName = x.CostCenter?.NameAr,
                    DocumentDateUtc = x.Expense?.ExpenseDateUtc ?? x.CreatedAtUtc,
                    DocumentNumber = x.Expense?.ExpenseNumber ?? string.Empty,
                    SourceKind = SourcePhaseExpense,
                    SourceKindAr = "مصروف مرحلة",
                    ExpenseItem = x.ProjectBudgetLine?.LineName ?? x.Expense?.Description ?? "مصروف مرحلة",
                    Amount = x.Expense?.Amount ?? 0m,
                    IncludeInActualCost = x.IncludeInActualCost,
                    Notes = x.Notes
                };
            }).ToList();
        }

        private async Task<List<GrantorFinancialStatementRowVm>> LoadStoreIssueRowsAsync(
            List<Guid> projectIds,
            List<ProjectFundingAgreement> agreements,
            DateTime? fromDate,
            DateTime? toDate,
            CancellationToken ct)
        {
            var query = _db.Set<ProjectPhaseStoreIssueLink>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.ProjectPhase)
                .Include(x => x.StoreIssue)
                    .ThenInclude(x => x.Lines)
                .Include(x => x.CostCenter)
                .Where(x => projectIds.Contains(x.ProjectId) && x.StoreIssue != null && x.Project != null);

            if (fromDate.HasValue)
                query = query.Where(x => x.StoreIssue!.IssueDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.StoreIssue!.IssueDate <= toDate.Value);

            var items = await query.ToListAsync(ct);

            return items.Select(x =>
            {
                var agreement = agreements.FirstOrDefault(a => a.ProjectId == x.ProjectId);

                return new GrantorFinancialStatementRowVm
                {
                    ProjectId = x.ProjectId,
                    ProjectName = x.Project?.Name ?? string.Empty,
                    GrantorName = agreement?.Grantor?.NameAr ?? string.Empty,
                    AgreementNumber = agreement?.AgreementNumber ?? string.Empty,
                    BudgetLineId = null,
                    BudgetLineName = "صرف مخزني",
                    PhaseName = x.ProjectPhase?.Name,
                    CostCenterName = x.CostCenter?.NameAr,
                    DocumentDateUtc = x.StoreIssue?.IssueDate ?? x.CreatedAtUtc,
                    DocumentNumber = x.StoreIssue?.IssueNumber ?? string.Empty,
                    SourceKind = SourceStoreIssue,
                    SourceKindAr = "صرف مخزني",
                    ExpenseItem = "صرف مخزني للمشروع",
                    Amount = x.StoreIssue?.Lines?.Sum(l => l.Quantity * l.UnitCost) ?? 0m,
                    IncludeInActualCost = x.IncludeInActualCost,
                    Notes = x.Notes
                };
            }).ToList();
        }

        private async Task<List<GrantorFinancialBudgetRowVm>> BuildBudgetRowsAsync(
            List<Guid> projectIds,
            List<GrantorFinancialStatementRowVm> statementRows,
            CancellationToken ct)
        {
            var budgetLines = await _db.Set<ProjectBudgetLine>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Where(x => projectIds.Contains(x.ProjectId))
                .OrderBy(x => x.Project!.Name)
                .ThenBy(x => x.LineName)
                .ToListAsync(ct);

            return budgetLines.Select(b =>
            {
                var actual = statementRows
                    .Where(r =>
                        r.IncludeInActualCost &&
                        r.ProjectId == b.ProjectId &&
                        r.BudgetLineId == b.Id)
                    .Sum(r => r.Amount);

                return new GrantorFinancialBudgetRowVm
                {
                    ProjectId = b.ProjectId,
                    ProjectName = b.Project?.Name ?? string.Empty,
                    BudgetLineId = b.Id,
                    BudgetLineName = b.LineName,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = actual
                };
            }).ToList();
        }

        private async Task FillLookupsAsync(GrantorFinancialReportPageVm vm, CancellationToken ct)
        {
            vm.Grantors = await _db.Set<Grantor>()
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.NameAr)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.NameAr,
                    Selected = vm.GrantorId.HasValue && vm.GrantorId.Value == x.Id
                })
                .ToListAsync(ct);

            vm.Projects = await _db.Set<CharityProject>()
                .AsNoTracking()
                .OrderBy(x => x.Name)
                .Select(x => new SelectListItem
                {
                    Value = x.Id.ToString(),
                    Text = x.Name,
                    Selected = vm.ProjectId.HasValue && vm.ProjectId.Value == x.Id
                })
                .ToListAsync(ct);
        }
    }
}
