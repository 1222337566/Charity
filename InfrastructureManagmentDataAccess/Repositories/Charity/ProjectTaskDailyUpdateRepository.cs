using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.TaskUpdates;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectTaskDailyUpdateRepository : IProjectTaskDailyUpdateRepository
    {
        private readonly AppDbContext _db;
        public ProjectTaskDailyUpdateRepository(AppDbContext db) => _db = db;

        public async Task<List<ProjectTaskDailyUpdateListItemVm>> GetByTaskIdAsync(Guid taskId)
        {
            return await _db.Set<ProjectTaskDailyUpdate>()
                .AsNoTracking()
                .Where(x => x.TaskId == taskId)
                .OrderByDescending(x => x.UpdateDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .Select(x => new ProjectTaskDailyUpdateListItemVm
                {
                    Id = x.Id,
                    TaskId = x.TaskId,
                    UpdateDate = x.UpdateDate,
                    Status = x.Status,
                    ProgressPercent = x.ProgressPercent,
                    HoursSpent = x.HoursSpent,
                    Note = x.Note,
                    BlockerNote = x.BlockerNote,
                    NextAction = x.NextAction,
                    CreatedByName = x.CreatedByName
                }).ToListAsync();
        }

        public Task<ProjectTaskDailyUpdate?> GetByIdAsync(Guid id)
            => _db.Set<ProjectTaskDailyUpdate>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectTaskDailyUpdate entity)
        {
            _db.Set<ProjectTaskDailyUpdate>().Add(entity);

            var task = await _db.Set<ProjectPhaseTask>().FirstOrDefaultAsync(x => x.Id == entity.TaskId);
            if (task != null)
            {
                task.PercentComplete = entity.ProgressPercent;
                task.SpentHours += entity.HoursSpent;
                task.Status = entity.Status;
                task.LastDailyUpdateAtUtc = DateTime.UtcNow;
                if (entity.Status == "InProgress" && task.StartedAtUtc == null)
                    task.StartedAtUtc = DateTime.UtcNow;
                if (entity.Status == "Done" && task.CompletedAtUtc == null)
                    task.CompletedAtUtc = DateTime.UtcNow;
            }

            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectTaskDailyUpdate entity)
        {
            entity.UpdatedAtUtc = DateTime.UtcNow;
            _db.Set<ProjectTaskDailyUpdate>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
