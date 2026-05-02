using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.ProjectFinancialControl;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Skote.Controllers
{
    [Authorize]
    public class ProjectFinancialControlReportsController : Controller
    {
        private const string SourceDirectExpense = "DirectExpense";
        private const string SourcePhaseExpense = "PhaseExpense";
        private const string SourceStoreIssue = "StoreIssue";

        private readonly AppDbContext _db;

        public ProjectFinancialControlReportsController(AppDbContext db)
        {
            _db = db;
        }

        public async Task<IActionResult> Index(
            Guid? projectId,
            DateTime? fromDate,
            DateTime? toDate,
            string? sourceKind,
            CancellationToken ct)
        {
            var vm = new ProjectFinancialControlPageVm
            {
                ProjectId = projectId,
                FromDate = fromDate,
                ToDate = toDate,
                SourceKind = sourceKind
            };

            await FillLookupsAsync(vm, ct);

            var projectsQuery = _db.Set<CharityProject>().AsNoTracking();

            if (projectId.HasValue)
                projectsQuery = projectsQuery.Where(x => x.Id == projectId.Value);

            var projects = await projectsQuery
                .OrderBy(x => x.Name)
                .Select(x => new
                {
                    x.Id,
                    x.Code,
                    x.Name,
                    x.Budget
                })
                .ToListAsync(ct);

            var statementRows = new List<ProjectExpenseStatementRowVm>();

            if (string.IsNullOrWhiteSpace(sourceKind) || sourceKind == SourceDirectExpense)
                statementRows.AddRange(await LoadDirectExpenseRowsAsync(projectId, fromDate, toDate, ct));

            if (string.IsNullOrWhiteSpace(sourceKind) || sourceKind == SourcePhaseExpense)
                statementRows.AddRange(await LoadPhaseExpenseRowsAsync(projectId, fromDate, toDate, ct));

            if (string.IsNullOrWhiteSpace(sourceKind) || sourceKind == SourceStoreIssue)
                statementRows.AddRange(await LoadStoreIssueRowsAsync(projectId, fromDate, toDate, ct));

            vm.StatementRows = statementRows
                .OrderByDescending(x => x.DocumentDateUtc)
                .ThenBy(x => x.ProjectName)
                .ToList();

            vm.Summaries = projects.Select(p =>
            {
                var rows = statementRows
                    .Where(r => r.ProjectId == p.Id && r.IncludeInActualCost)
                    .ToList();

                return new ProjectFinancialSummaryRowVm
                {
                    ProjectId = p.Id,
                    ProjectCode = p.Code,
                    ProjectName = p.Name,
                    ProjectBudget = p.Budget,
                    DirectCashExpenses = rows.Where(r => r.SourceKind == SourceDirectExpense).Sum(r => r.Amount),
                    PhaseCashExpenses = rows.Where(r => r.SourceKind == SourcePhaseExpense).Sum(r => r.Amount),
                    StoreIssueExpenses = rows.Where(r => r.SourceKind == SourceStoreIssue).Sum(r => r.Amount)
                };
            }).ToList();

            vm.BudgetRows = await BuildBudgetUtilizationRowsAsync(projectId, statementRows, ct);

            return View(vm);
        }

        private async Task<List<ProjectExpenseStatementRowVm>> LoadDirectExpenseRowsAsync(
            Guid? projectId,
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
                .Where(x => x.Expense != null && x.Project != null);

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);

            if (fromDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc <= toDate.Value);

            var items = await query.ToListAsync(ct);

            return items.Select(x => new ProjectExpenseStatementRowVm
            {
                ProjectId = x.ProjectId,
                ProjectName = x.Project?.Name ?? string.Empty,
                PhaseId = null,
                PhaseName = null,
                BudgetLineId = x.ProjectBudgetLineId,
                BudgetLineName = x.ProjectBudgetLine?.LineName,
                CostCenterName = x.CostCenter?.NameAr,
                DocumentDateUtc = x.Expense?.ExpenseDateUtc ?? x.CreatedAtUtc,
                DocumentNumber = x.Expense?.ExpenseNumber ?? string.Empty,
                SourceKind = SourceDirectExpense,
                SourceKindAr = "مصروف مباشر",
                ExpenseItem = x.ProjectBudgetLine?.LineName ?? x.Expense?.Description ?? "مصروف",
                Amount = x.Expense?.Amount ?? 0m,
                IncludeInActualCost = true,
                Notes = x.Notes
            }).ToList();
        }

        private async Task<List<ProjectExpenseStatementRowVm>> LoadPhaseExpenseRowsAsync(
            Guid? projectId,
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
                .Where(x => x.Expense != null && x.Project != null);

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);

            if (fromDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.Expense!.ExpenseDateUtc <= toDate.Value);

            var items = await query.ToListAsync(ct);

            return items.Select(x => new ProjectExpenseStatementRowVm
            {
                ProjectId = x.ProjectId,
                ProjectName = x.Project?.Name ?? string.Empty,
                PhaseId = x.ProjectPhaseId,
                PhaseName = x.ProjectPhase?.Name,
                BudgetLineId = x.ProjectBudgetLineId,
                BudgetLineName = x.ProjectBudgetLine?.LineName,
                CostCenterName = x.CostCenter?.NameAr,
                DocumentDateUtc = x.Expense?.ExpenseDateUtc ?? x.CreatedAtUtc,
                DocumentNumber = x.Expense?.ExpenseNumber ?? string.Empty,
                SourceKind = SourcePhaseExpense,
                SourceKindAr = "مصروف مرحلة",
                ExpenseItem = x.ProjectBudgetLine?.LineName ?? x.Expense?.Description ?? "مصروف مرحلة",
                Amount = x.Expense?.Amount ?? 0m,
                IncludeInActualCost = x.IncludeInActualCost,
                Notes = x.Notes
            }).ToList();
        }

        private async Task<List<ProjectExpenseStatementRowVm>> LoadStoreIssueRowsAsync(
            Guid? projectId,
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
                .Where(x => x.StoreIssue != null && x.Project != null);

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);

            if (fromDate.HasValue)
                query = query.Where(x => x.StoreIssue!.IssueDate >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(x => x.StoreIssue!.IssueDate <= toDate.Value);

            var items = await query.ToListAsync(ct);

            return items.Select(x => new ProjectExpenseStatementRowVm
            {
                ProjectId = x.ProjectId,
                ProjectName = x.Project?.Name ?? string.Empty,
                PhaseId = x.ProjectPhaseId,
                PhaseName = x.ProjectPhase?.Name,
                BudgetLineId = null,
                BudgetLineName = "صرف مخزني",
                CostCenterName = x.CostCenter?.NameAr,
                DocumentDateUtc = x.StoreIssue?.IssueDate ?? x.CreatedAtUtc,
                DocumentNumber = x.StoreIssue?.IssueNumber ?? string.Empty,
                SourceKind = SourceStoreIssue,
                SourceKindAr = "صرف مخزني",
                ExpenseItem = "صرف مخزني للمشروع",
                Amount = x.StoreIssue?.Lines?.Sum(l => l.Quantity * l.UnitCost) ?? 0m,
                IncludeInActualCost = x.IncludeInActualCost,
                Notes = x.Notes
            }).ToList();
        }

        private async Task<List<ProjectBudgetUtilizationRowVm>> BuildBudgetUtilizationRowsAsync(
            Guid? projectId,
            List<ProjectExpenseStatementRowVm> statementRows,
            CancellationToken ct)
        {
            var query = _db.Set<ProjectBudgetLine>()
                .AsNoTracking()
                .Include(x => x.Project)
                .AsQueryable();

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);

            var budgetLines = await query
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

                return new ProjectBudgetUtilizationRowVm
                {
                    ProjectId = b.ProjectId,
                    BudgetLineId = b.Id,
                    ProjectName = b.Project?.Name ?? string.Empty,
                    BudgetLineName = b.LineName,
                    LineType = b.LineType,
                    PlannedAmount = b.PlannedAmount,
                    ActualAmount = actual
                };
            }).ToList();
        }

        private async Task FillLookupsAsync(ProjectFinancialControlPageVm vm, CancellationToken ct)
        {
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

            vm.SourceKinds = new List<SelectListItem>
            {
                new() { Value = "", Text = "كل المصادر", Selected = string.IsNullOrWhiteSpace(vm.SourceKind) },
                new() { Value = SourceDirectExpense, Text = "مصروف مباشر", Selected = vm.SourceKind == SourceDirectExpense },
                new() { Value = SourcePhaseExpense, Text = "مصروف مرحلة", Selected = vm.SourceKind == SourcePhaseExpense },
                new() { Value = SourceStoreIssue, Text = "صرف مخزني", Selected = vm.SourceKind == SourceStoreIssue }
            };
        }
    }
}
