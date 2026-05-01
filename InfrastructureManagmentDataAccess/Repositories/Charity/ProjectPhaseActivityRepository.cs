using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects;
using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Activities;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class ProjectPhaseActivityRepository : IProjectPhaseActivityRepository
    {
        private readonly AppDbContext _db;
        public ProjectPhaseActivityRepository(AppDbContext db) => _db = db;

        public async Task<List<ProjectPhaseActivityListItemVm>> GetByPhaseIdAsync(Guid phaseId)
        {
            return await _db.Set<ProjectPhaseActivity>()
                .AsNoTracking()
                .Where(x => x.PhaseId == phaseId)
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Title)
                .Select(x => new ProjectPhaseActivityListItemVm
                {
                    Id = x.Id,
                    ProjectId = x.ProjectId,
                    PhaseId = x.PhaseId,
                    Code = x.Code,
                    Title = x.Title,
                    Status = x.Status,
                    Priority = x.Priority,
                    ProgressPercent = x.ProgressPercent,
                    PlannedHours = x.PlannedHours,
                    ActualHours = x.ActualHours,
                    ResponsiblePersonName = x.ResponsiblePersonName,
                    PlannedStartDate = x.PlannedStartDate,
                    PlannedEndDate = x.PlannedEndDate,
                    OpenTasksCount = _db.Set<ProjectPhaseTask>().Count(t => t.ActivityId == x.Id && t.IsActive && t.Status != "Done")
                }).ToListAsync();
        }

        public Task<ProjectPhaseActivity?> GetByIdAsync(Guid id)
            => _db.Set<ProjectPhaseActivity>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(ProjectPhaseActivity entity)
        {
            _db.Set<ProjectPhaseActivity>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ProjectPhaseActivity entity)
        {
            entity.UpdatedAtUtc = DateTime.UtcNow;
            _db.Set<ProjectPhaseActivity>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
