using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectPlanning.Tasks;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectTaskTrackingRepository : IProjectTaskTrackingRepository
    {
        private readonly AppDbContext _db;
        public ProjectTaskTrackingRepository(AppDbContext db) => _db = db;

        public async Task<ProjectTaskBoardVm?> BuildBoardAsync(Guid projectId)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return null;

            var tasks = await _db.Set<ProjectPhaseTask>()
                .AsNoTracking()
                .Where(x => x.ProjectId == projectId && x.IsActive)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
                .Select(x => new ProjectPhaseTaskListItemVm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    PhaseId = x.PhaseId,
                    ActivityId = x.ActivityId,
                    Code = x.Code,
                    Title = x.Title,
                    Status = x.Status,
                    Priority = x.Priority,
                    PercentComplete = x.PercentComplete,
                    EstimatedHours = x.EstimatedHours,
                    SpentHours = x.SpentHours,
                    AssignedToName = x.AssignedToName,
                    DueDate = x.DueDate,
                    LastDailyUpdateAtUtc = x.LastDailyUpdateAtUtc
                }).ToListAsync();

            var statuses = new[] { "Todo", "InProgress", "Blocked", "Done" };
            var vm = new ProjectTaskBoardVm
            {
                ProjectHeader = new ProjectHeaderVm
                {
                    Id = project.Id,
                    Code = project.Code,
                    Name = project.Name,
                    Status = project.Status,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Budget = project.Budget
                },
                OverdueCount = tasks.Count(x => x.Status != "Done" && x.DueDate.HasValue && x.DueDate.Value.Date < DateTime.Today),
                NeedsFollowUpCount = tasks.Count(x => x.Status != "Done" && (!x.LastDailyUpdateAtUtc.HasValue || x.LastDailyUpdateAtUtc.Value.Date < DateTime.Today))
            };
            vm.Columns = statuses.Select(s => new ProjectTaskBoardColumnVm { Status = s, Tasks = tasks.Where(x => x.Status == s).ToList() }).ToList();
            return vm;
        }

        public async Task<ProjectTaskDailyFollowUpVm?> BuildDailyFollowUpAsync(Guid projectId, DateTime date)
        {
            var project = await _db.Set<CharityProject>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == projectId);
            if (project == null) return null;

            var rows = await (from t in _db.Set<ProjectPhaseTask>().AsNoTracking()
                              join a in _db.Set<ProjectPhaseActivity>().AsNoTracking() on t.ActivityId equals a.Id
                              join p in _db.Set<ProjectPhase>().AsNoTracking() on t.PhaseId equals p.Id
                              where t.ProjectId == projectId && t.IsActive
                              select new ProjectTaskDailyFollowUpRowVm
                              {
                                  TaskId = t.Id,
                                  TaskTitle = t.Title,
                                  PhaseName = p.Name,
                                  ActivityTitle = a.Title,
                                  Status = t.Status,
                                  PercentComplete = t.PercentComplete,
                                  AssignedToName = t.AssignedToName,
                                  TodayHours = _db.Set<ProjectTaskDailyUpdate>().Where(u => u.TaskId == t.Id && u.UpdateDate.Date == date.Date).Select(u => (decimal?)u.HoursSpent).Sum() ?? 0,
                                  TodayNote = _db.Set<ProjectTaskDailyUpdate>().Where(u => u.TaskId == t.Id && u.UpdateDate.Date == date.Date).OrderByDescending(u => u.CreatedAtUtc).Select(u => u.Note).FirstOrDefault(),
                                  MissingFollowUp = t.RequiresDailyFollowUp && !_db.Set<ProjectTaskDailyUpdate>().Any(u => u.TaskId == t.Id && u.UpdateDate.Date == date.Date)
                              }).OrderBy(x => x.PhaseName).ThenBy(x => x.ActivityTitle).ThenBy(x => x.TaskTitle).ToListAsync();

            return new ProjectTaskDailyFollowUpVm
            {
                ProjectHeader = new ProjectHeaderVm
                {
                    Id = project.Id,
                    Code = project.Code,
                    Name = project.Name,
                    Status = project.Status,
                    StartDate = project.StartDate,
                    EndDate = project.EndDate,
                    Budget = project.Budget
                },
                Date = date.Date,
                Rows = rows
            };
        }
    }
}
