using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ProjectPhaseAccountingReportRepository : IProjectPhaseAccountingReportRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseAccountingReportRepository(AppDbContext db) => _db = db;

        public async Task<ProjectPhaseCostSummaryDto> GetPhaseCostSummaryAsync(Guid? projectId, Guid? phaseId, DateTime? fromDate, DateTime? toDate)
        {
            var phaseQuery = _db.Set<ProjectPhase>().AsNoTracking().Include(x => x.Project).Include(x => x.Milestones).AsQueryable();
            if (projectId.HasValue) phaseQuery = phaseQuery.Where(x => x.ProjectId == projectId.Value);
            if (phaseId.HasValue) phaseQuery = phaseQuery.Where(x => x.Id == phaseId.Value);

            var phases = await phaseQuery.OrderBy(x => x.Project!.Name).ThenBy(x => x.SortOrder).ThenBy(x => x.Name).ToListAsync();
            var phaseIds = phases.Select(x => x.Id).ToList();

            var expenseLinks = await _db.Set<ProjectPhaseExpenseLink>()
                .AsNoTracking()
                .Include(x => x.Expense)
                .Where(x => phaseIds.Contains(x.ProjectPhaseId) && x.IncludeInActualCost)
                .ToListAsync();

            var storeLinks = await _db.Set<ProjectPhaseStoreIssueLink>()
                .AsNoTracking()
                .Include(x => x.StoreIssue)!.ThenInclude(x => x!.Lines)
                .Where(x => phaseIds.Contains(x.ProjectPhaseId) && x.IncludeInActualCost)
                .ToListAsync();

            if (fromDate.HasValue)
            {
                expenseLinks = expenseLinks.Where(x => x.Expense != null && x.Expense.ExpenseDateUtc.Date >= fromDate.Value.Date).ToList();
                storeLinks = storeLinks.Where(x => x.StoreIssue != null && x.StoreIssue.IssueDate.Date >= fromDate.Value.Date).ToList();
            }
            if (toDate.HasValue)
            {
                expenseLinks = expenseLinks.Where(x => x.Expense != null && x.Expense.ExpenseDateUtc.Date <= toDate.Value.Date).ToList();
                storeLinks = storeLinks.Where(x => x.StoreIssue != null && x.StoreIssue.IssueDate.Date <= toDate.Value.Date).ToList();
            }

            var rows = phases.Select(phase =>
            {
                var expenseAmount = expenseLinks.Where(x => x.ProjectPhaseId == phase.Id).Sum(x => x.Expense?.Amount ?? 0m);
                var storeAmount = storeLinks.Where(x => x.ProjectPhaseId == phase.Id)
                    .Sum(x => x.StoreIssue?.Lines?.Sum(l => l.Quantity * l.UnitCost) ?? 0m);
                var actualCost = expenseAmount + storeAmount;

                return new ProjectPhaseCostSummaryRowDto
                {
                    ProjectId = phase.ProjectId,
                    ProjectName = phase.Project?.Name ?? string.Empty,
                    PhaseId = phase.Id,
                    PhaseName = phase.Name,
                    PhaseStatus = phase.Status,
                    ProgressPercent = phase.ProgressPercent,
                    PlannedCost = phase.PlannedCost,
                    ActualCost = actualCost > 0 ? actualCost : phase.ActualCost,
                    MilestonesCount = phase.Milestones.Count,
                    CompletedMilestonesCount = phase.Milestones.Count(x => x.CompletedDate.HasValue || x.Status == "Completed")
                };
            }).ToList();

            return new ProjectPhaseCostSummaryDto { FromDate = fromDate, ToDate = toDate, Rows = rows };
        }

        public async Task<ProjectPhaseLedgerDto?> GetPhaseLedgerAsync(Guid phaseId, DateTime? fromDate, DateTime? toDate)
        {
            var phase = await _db.Set<ProjectPhase>().AsNoTracking().Include(x => x.Project).FirstOrDefaultAsync(x => x.Id == phaseId);
            if (phase == null) return null;

            var expenseLinksQuery = _db.Set<ProjectPhaseExpenseLink>()
                .AsNoTracking()
                .Include(x => x.Expense)
                .Include(x => x.ProjectBudgetLine)
                .Include(x => x.CostCenter)
                .Where(x => x.ProjectPhaseId == phaseId);

            var storeLinksQuery = _db.Set<ProjectPhaseStoreIssueLink>()
                .AsNoTracking()
                .Include(x => x.StoreIssue)!.ThenInclude(x => x!.Lines)
                .Include(x => x.CostCenter)
                .Where(x => x.ProjectPhaseId == phaseId);

            if (fromDate.HasValue)
            {
                expenseLinksQuery = expenseLinksQuery.Where(x => x.Expense != null && x.Expense.ExpenseDateUtc.Date >= fromDate.Value.Date);
                storeLinksQuery = storeLinksQuery.Where(x => x.StoreIssue != null && x.StoreIssue.IssueDate.Date >= fromDate.Value.Date);
            }
            if (toDate.HasValue)
            {
                expenseLinksQuery = expenseLinksQuery.Where(x => x.Expense != null && x.Expense.ExpenseDateUtc.Date <= toDate.Value.Date);
                storeLinksQuery = storeLinksQuery.Where(x => x.StoreIssue != null && x.StoreIssue.IssueDate.Date <= toDate.Value.Date);
            }

            var rows = new List<ProjectPhaseLedgerRowDto>();
            rows.AddRange(await expenseLinksQuery.Select(x => new ProjectPhaseLedgerRowDto
            {
                SourceType = "Expense",
                SourceId = x.ExpenseId,
                ReferenceNumber = x.Expense != null ? x.Expense.ExpenseNumber : string.Empty,
                EntryDate = x.Expense != null ? x.Expense.ExpenseDateUtc : x.CreatedAtUtc,
                Description = x.Expense != null ? x.Expense.Description : string.Empty,
                BudgetLineName = x.ProjectBudgetLine != null ? x.ProjectBudgetLine.LineName : null,
                CostCenterName = x.CostCenter != null ? x.CostCenter.NameAr : null,
                Amount = x.Expense != null ? x.Expense.Amount : 0m
            }).ToListAsync());

            var storeLinks = await storeLinksQuery.ToListAsync();
            rows.AddRange(storeLinks.Select(x => new ProjectPhaseLedgerRowDto
            {
                SourceType = "StoreIssue",
                SourceId = x.StoreIssueId,
                ReferenceNumber = x.StoreIssue?.IssueNumber ?? string.Empty,
                EntryDate = x.StoreIssue?.IssueDate ?? x.CreatedAtUtc,
                Description = x.StoreIssue?.Notes ?? "صرف مخزني مرتبط بمرحلة",
                BudgetLineName = null,
                CostCenterName = x.CostCenter?.NameAr,
                Amount = x.StoreIssue?.Lines?.Sum(l => l.Quantity * l.UnitCost) ?? 0m
            }));

            rows = rows.OrderBy(x => x.EntryDate).ThenBy(x => x.ReferenceNumber).ToList();
            return new ProjectPhaseLedgerDto
            {
                ProjectId = phase.ProjectId,
                ProjectName = phase.Project?.Name ?? string.Empty,
                PhaseId = phase.Id,
                PhaseName = phase.Name,
                FromDate = fromDate,
                ToDate = toDate,
                TotalAmount = rows.Sum(x => x.Amount),
                Rows = rows
            };
        }

        public async Task<ProjectPhaseAlertDto> GetAlertsAsync(Guid? projectId, DateTime? asOfDate)
        {
            var today = (asOfDate ?? DateTime.Today).Date;
            var phases = await _db.Set<ProjectPhase>()
                .AsNoTracking()
                .Include(x => x.Project)
                .Include(x => x.Milestones)
                .Where(x => !projectId.HasValue || x.ProjectId == projectId.Value)
                .OrderBy(x => x.Project!.Name).ThenBy(x => x.SortOrder)
                .ToListAsync();

            var costSummary = await GetPhaseCostSummaryAsync(projectId, null, null, today);
            var costMap = costSummary.Rows.ToDictionary(x => x.PhaseId, x => x);
            var rows = new List<ProjectPhaseAlertRowDto>();

            foreach (var phase in phases)
            {
                if (phase.PlannedEndDate.Date < today && phase.ProgressPercent < 100 && phase.Status != "Completed")
                {
                    rows.Add(new ProjectPhaseAlertRowDto
                    {
                        ProjectId = phase.ProjectId,
                        ProjectName = phase.Project?.Name ?? string.Empty,
                        PhaseId = phase.Id,
                        PhaseName = phase.Name,
                        AlertType = "Delay",
                        Severity = "High",
                        Message = "المرحلة تجاوزت تاريخ النهاية المخطط ولم تكتمل بعد",
                        RelatedDate = phase.PlannedEndDate
                    });
                }

                if (phase.PlannedEndDate.Date.AddDays(-7) <= today && phase.ProgressPercent < 75 && phase.Status != "Completed")
                {
                    rows.Add(new ProjectPhaseAlertRowDto
                    {
                        ProjectId = phase.ProjectId,
                        ProjectName = phase.Project?.Name ?? string.Empty,
                        PhaseId = phase.Id,
                        PhaseName = phase.Name,
                        AlertType = "LowProgress",
                        Severity = "Medium",
                        Message = "المرحلة تقترب من نهايتها المخططة مع نسبة إنجاز منخفضة",
                        RelatedDate = phase.PlannedEndDate
                    });
                }

                if (costMap.TryGetValue(phase.Id, out var cost) && cost.ActualCost > cost.PlannedCost && cost.PlannedCost > 0)
                {
                    rows.Add(new ProjectPhaseAlertRowDto
                    {
                        ProjectId = phase.ProjectId,
                        ProjectName = phase.Project?.Name ?? string.Empty,
                        PhaseId = phase.Id,
                        PhaseName = phase.Name,
                        AlertType = "CostOverrun",
                        Severity = "High",
                        Message = "التكلفة الفعلية للمرحلة تجاوزت المخطط",
                        PlannedCost = cost.PlannedCost,
                        ActualCost = cost.ActualCost
                    });
                }

                foreach (var milestone in phase.Milestones.Where(m => !m.CompletedDate.HasValue && m.DueDate.Date < today && m.Status != "Completed"))
                {
                    rows.Add(new ProjectPhaseAlertRowDto
                    {
                        ProjectId = phase.ProjectId,
                        ProjectName = phase.Project?.Name ?? string.Empty,
                        PhaseId = phase.Id,
                        PhaseName = phase.Name,
                        AlertType = "MilestoneOverdue",
                        Severity = "Medium",
                        Message = $"Milestone متأخرة: {milestone.Title}",
                        RelatedDate = milestone.DueDate
                    });
                }
            }

            return new ProjectPhaseAlertDto { AsOfDate = today, Rows = rows.OrderByDescending(x => x.Severity).ThenBy(x => x.ProjectName).ThenBy(x => x.PhaseName).ToList() };
        }
    }
}
