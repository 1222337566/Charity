using InfrastrfuctureManagmentCore.Domains.Accounting;
using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories
{
    public class ProjectAccountingReportRepository : IProjectAccountingReportRepository
    {
        private readonly AppDbContext _db;
        public ProjectAccountingReportRepository(AppDbContext db) => _db = db;

        public async Task<ProjectCostSummaryDto> GetProjectCostSummaryAsync(Guid? projectId, Guid? costCenterId, DateTime? fromDate, DateTime? toDate)
        {
            var query = _db.Set<JournalEntryLine>()
                .AsNoTracking()
                .Include(x => x.JournalEntry)
                .Include(x => x.CostCenter)
                .Where(x => x.ProjectId != null && x.JournalEntry != null && x.JournalEntry.Status == JournalEntryStatus.Posted);

            if (projectId.HasValue)
                query = query.Where(x => x.ProjectId == projectId.Value);
            if (costCenterId.HasValue)
                query = query.Where(x => x.CostCenterId == costCenterId.Value);
            if (fromDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                query = query.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            var projects = await _db.Set<CharityProject>().AsNoTracking().ToDictionaryAsync(x => x.Id, x => x.Name);
            var lines = await query.ToListAsync();

            var rows = lines
                .GroupBy(x => new { x.ProjectId, CostCenterName = x.CostCenter != null ? x.CostCenter.NameAr : null })
                .Select(g => new ProjectCostSummaryRowDto
                {
                    ProjectId = g.Key.ProjectId,
                    ProjectName = g.Key.ProjectId.HasValue && projects.ContainsKey(g.Key.ProjectId.Value) ? projects[g.Key.ProjectId.Value] : "بدون مشروع",
                    CostCenterName = (string)g.Key.CostCenterName,
                    TotalDebit = g.Sum(x => x.DebitAmount),
                    TotalCredit = g.Sum(x => x.CreditAmount)
                })
                .OrderBy(x => x.ProjectName)
                .ThenBy(x => x.CostCenterName)
                .ToList();

            return new ProjectCostSummaryDto { FromDate = fromDate, ToDate = toDate, Rows = rows };
        }

        public async Task<ProjectLedgerDto?> GetProjectLedgerAsync(Guid projectId, DateTime? fromDate, DateTime? toDate)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return null;

            var baseQuery = _db.Set<JournalEntryLine>()
                .AsNoTracking()
                .Include(x => x.JournalEntry)
                .Include(x => x.FinancialAccount)
                .Include(x => x.CostCenter)
                .Where(x => x.ProjectId == projectId && x.JournalEntry != null && x.JournalEntry.Status == JournalEntryStatus.Posted);

            decimal openingBalance = 0m;
            if (fromDate.HasValue)
            {
                openingBalance = await baseQuery
                    .Where(x => x.JournalEntry!.EntryDate.Date < fromDate.Value.Date)
                    .SumAsync(x => x.DebitAmount - x.CreditAmount);
            }

            if (fromDate.HasValue)
                baseQuery = baseQuery.Where(x => x.JournalEntry!.EntryDate.Date >= fromDate.Value.Date);
            if (toDate.HasValue)
                baseQuery = baseQuery.Where(x => x.JournalEntry!.EntryDate.Date <= toDate.Value.Date);

            var ordered = await baseQuery.OrderBy(x => x.JournalEntry!.EntryDate).ThenBy(x => x.JournalEntry!.EntryNumber).ToListAsync();
            var running = openingBalance;
            var rows = new List<ProjectLedgerRowDto>();
            foreach (var line in ordered)
            {
                running += line.DebitAmount - line.CreditAmount;
                rows.Add(new ProjectLedgerRowDto
                {
                    EntryDate = line.JournalEntry!.EntryDate,
                    EntryNumber = line.JournalEntry.EntryNumber,
                    AccountName = line.FinancialAccount?.AccountNameAr ?? string.Empty,
                    CostCenterName = (string)(line.CostCenter?.NameAr),
                    Description = line.Description ?? line.JournalEntry.Description,
                    SourceType = line.JournalEntry.SourceType,
                    DebitAmount = line.DebitAmount,
                    CreditAmount = line.CreditAmount,
                    RunningBalance = running
                });
            }

            return new ProjectLedgerDto
            {
                ProjectId = projectId,
                ProjectName = project.Name,
                FromDate = fromDate,
                ToDate = toDate,
                OpeningBalance = openingBalance,
                Rows = rows
            };
        }
    }
}
