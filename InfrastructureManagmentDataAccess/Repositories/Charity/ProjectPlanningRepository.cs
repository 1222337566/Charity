using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tracking;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectPlanningRepository : IProjectPlanningRepository
    {
        private readonly AppDbContext _db;
        public ProjectPlanningRepository(AppDbContext db) => _db = db;

        public async Task<ProjectPlanningCalendarVm?> BuildCalendarAsync(Guid projectId, int year, int month)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return null;

            var phases = await _db.Set<ProjectPhase>().AsNoTracking()
                .Where(x => x.ProjectId == projectId && x.IsActive)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.PlannedStartDate)
                .ToListAsync();

            var milestones = await _db.Set<ProjectPhaseMilestone>().AsNoTracking()
                .Where(x => x.Phase != null && x.Phase.ProjectId == projectId && x.IsActive)
                .Select(x => new { x.Id, x.ProjectPhaseId, x.Title, x.DueDate, x.CompletedDate, x.Status, PhaseName = x.Phase!.Name })
                .ToListAsync();

            var firstDay = new DateTime(year, month, 1);
            var startGrid = firstDay.AddDays(-(int)firstDay.DayOfWeek);
            var days = new List<ProjectPlanningCalendarDayVm>();
            for (var i = 0; i < 42; i++)
            {
                var day = startGrid.AddDays(i);
                var dayPhases = phases.Where(x => day.Date >= x.PlannedStartDate.Date && day.Date <= x.PlannedEndDate.Date)
                    .Select(x => new ProjectPlanningCalendarItemVm
                    {
                        Kind = "Phase",
                        ReferenceId = x.Id,
                        Title = x.Name,
                        Status = x.Status,
                        StartDate = x.PlannedStartDate,
                        EndDate = x.PlannedEndDate,
                        CssClass = GetCssClass(x.Status)
                    }).ToList();

                var dayMilestones = milestones.Where(x => x.DueDate.Date == day.Date)
                    .Select(x => new ProjectPlanningCalendarItemVm
                    {
                        Kind = "Milestone",
                        ReferenceId = x.Id,
                        Title = x.Title,
                        Status = x.Status,
                        StartDate = x.DueDate,
                        EndDate = x.DueDate,
                        Subtitle = x.PhaseName,
                        CssClass = x.Status == "Completed" ? "success" : "warning"
                    }).ToList();

                days.Add(new ProjectPlanningCalendarDayVm
                {
                    Date = day,
                    InCurrentMonth = day.Month == month,
                    Items = dayPhases.Concat(dayMilestones).OrderBy(x => x.Kind).ThenBy(x => x.Title).ToList()
                });
            }

            return new ProjectPlanningCalendarVm
            {
                ProjectHeader = new ProjectHeaderVm
                {
                    Id = project.Id,
                    Code = project.Code,
                    Name = project.Name,
                    Status = project.Status,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Budget = project.Budget,
                    IsActive = project.IsActive
                },
                Year = year,
                Month = month,
                MonthName = firstDay.ToString("MMMM yyyy"),
                Days = days
            };
        }

        public async Task<ProjectPlanningTimelineVm?> BuildTimelineAsync(Guid projectId)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return null;

            var phases = await _db.Set<ProjectPhase>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.PlannedStartDate)
                .ToListAsync();

            if (phases.Count == 0)
            {
                return new ProjectPlanningTimelineVm
                {
                    ProjectHeader = new ProjectHeaderVm { Id = project.Id, Code = project.Code, Name = project.Name, Status = project.Status, StartDate = project.StartDate, EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive },
                    RangeStart = project.StartDate.Date,
                    RangeEnd = (project.EndDate ?? project.StartDate).Date,
                    Rows = new List<ProjectPlanningTimelineRowVm>()
                };
            }

            var rangeStart = phases.Min(x => x.PlannedStartDate).Date;
            var rangeEnd = phases.Max(x => x.PlannedEndDate).Date;
            var totalDays = Math.Max(1, (rangeEnd - rangeStart).Days + 1);

            return new ProjectPlanningTimelineVm
            {
                ProjectHeader = new ProjectHeaderVm { Id = project.Id, Code = project.Code, Name = project.Name, Status = project.Status, StartDate = project.StartDate, EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive },
                RangeStart = rangeStart,
                RangeEnd = rangeEnd,
                Rows = phases.Select(x =>
                {
                    var offset = (int)(x.PlannedStartDate.Date - rangeStart).TotalDays;
                    var width = Math.Max(1, (int)(x.PlannedEndDate.Date - x.PlannedStartDate.Date).TotalDays + 1);
                    return new ProjectPlanningTimelineRowVm
                    {
                        PhaseId = x.Id,
                        PhaseName = x.Name,
                        Status = x.Status,
                        ProgressPercent = x.ProgressPercent,
                        StartDate = x.PlannedStartDate,
                        EndDate = x.PlannedEndDate,
                        OffsetPercent = Math.Round((decimal)offset / totalDays * 100m, 2),
                        WidthPercent = Math.Round((decimal)width / totalDays * 100m, 2)
                    };
                }).ToList()
            };
        }

        public async Task<ProjectTrackingBoardVm?> BuildTrackingBoardAsync(Guid projectId)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return null;

            var phases = await _db.Set<ProjectPhase>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

            var milestones = await _db.Set<ProjectPhaseMilestone>().AsNoTracking()
                .Where(x => x.Phase != null && x.Phase.ProjectId == projectId && x.IsActive)
                .OrderBy(x => x.DueDate)
                .Select(x => new ProjectMilestoneSummaryVm
                {
                    Id = x.Id,
                    PhaseName = x.Phase!.Name,
                    Title = x.Title,
                    DueDate = x.DueDate,
                    CompletedDate = x.CompletedDate,
                    Status = x.Status
                })
                .ToListAsync();

            var activities = await _db.Set<ProjectActivity>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderBy(x => x.PlannedDate)
                .Select(x => new ProjectActivitySummaryVm
                {
                    Id = x.Id,
                    Title = x.Title,
                    PlannedDate = x.PlannedDate,
                    ActualDate = x.ActualDate,
                    Status = x.Status,
                    PlannedCost = x.PlannedCost,
                    ActualCost = x.ActualCost
                })
                .ToListAsync();

            var budgetLines = await _db.Set<ProjectBudgetLine>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .ToListAsync();

            var logs = await _db.Set<ProjectTrackingLog>().AsNoTracking()
                .Where(x => x.ProjectId == projectId)
                .OrderByDescending(x => x.EntryDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Take(50)
                .ToListAsync();

            return new ProjectTrackingBoardVm
            {
                ProjectHeader = new ProjectHeaderVm
                {
                    Id = project.Id, Code = project.Code, Name = project.Name,
                    Status = project.Status, StartDate = project.StartDate,
                    EndDate = project.EndDate, Budget = project.Budget, IsActive = project.IsActive
                },
                TotalPlannedBudget = budgetLines.Sum(x => x.PlannedAmount),
                TotalActualSpent   = budgetLines.Sum(x => x.ActualAmount),
                Milestones  = milestones,
                Activities  = activities,
                PhaseSummaries = phases.Select(x => new ProjectTrackingPhaseSummaryVm
                {
                    PhaseId = x.Id,
                    PhaseName = x.Name,
                    Status = x.Status,
                    ProgressPercent = x.ProgressPercent,
                    PlannedStartDate = x.PlannedStartDate,
                    PlannedEndDate   = x.PlannedEndDate,
                    ActualStartDate  = x.ActualStartDate,
                    ActualEndDate    = x.ActualEndDate,
                    RequiresAttention = x.Status == "Delayed"
                        || (x.PlannedEndDate.Date < DateTime.Today && x.ProgressPercent < 100m)
                }).ToList(),
                RecentLogs = logs.Select(x => new ProjectTrackingLogListItemVm
                {
                    Id = x.Id,
                    ProjectPhaseId = x.ProjectPhaseId,
                    EntryDate = x.EntryDate,
                    EntryType = x.EntryType,
                    Title = x.Title,
                    Details = x.Details,
                    Status = x.Status,
                    ProgressPercent = x.ProgressPercent,
                    RequiresAttention = x.RequiresAttention
                }).ToList()
            };
        }

        private static string GetCssClass(string status)
            => status switch
            {
                "Completed" => "success",
                "InProgress" => "primary",
                "Delayed" => "danger",
                "OnHold" => "secondary",
                _ => "info"
            };
    }
}
