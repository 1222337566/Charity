using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Tasks;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectPhaseTaskRepository : IProjectPhaseTaskRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseTaskRepository(AppDbContext db) => _db = db;

        public async Task<List<ProjectPhaseTaskListItemVm>> GetByActivityIdAsync(Guid activityId)
        {
            return await _db.Set<ProjectPhaseTask>()
                .AsNoTracking()
                .Where(x => x.ActivityId == activityId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
                .Select(Map()).ToListAsync();
        }

        public async Task<List<ProjectPhaseTaskListItemVm>> GetByPhaseIdAsync(Guid phaseId)
        {
            return await _db.Set<ProjectPhaseTask>()
                .AsNoTracking()
                .Where(x => x.PhaseId == phaseId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
                .Select(Map()).ToListAsync();
        }

        public Task<ProjectPhaseTask?> GetByIdAsync(Guid id)
            => _db.Set<ProjectPhaseTask>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectPhaseTask entity)
        {
            _db.Set<ProjectPhaseTask>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectPhaseTask entity)
        {
            entity.UpdatedAtUtc = DateTime.UtcNow;
            _db.Set<ProjectPhaseTask>().Update(entity);
            await _db.SaveChangesAsync();
        }

        private static System.Linq.Expressions.Expression<Func<ProjectPhaseTask, ProjectPhaseTaskListItemVm>> Map()
            => x => new ProjectPhaseTaskListItemVm
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
            };
    }
}
