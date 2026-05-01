using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerDirectoryRepository : IVolunteerDirectoryRepository
    {
        private readonly AppDbContext _db;
        public VolunteerDirectoryRepository(AppDbContext db) => _db = db;

        public async Task<List<Volunteer>> SearchAsync(string? q, Guid? skillDefinitionId, string? area, bool activeOnly = true)
        {
            var query = _db.Set<Volunteer>()
                .AsNoTracking()
                .Include(x => x.Assignments)
                .Include(x => x.HourLogs)
                .AsQueryable();

            if (activeOnly)
                query = query.Where(x => x.IsActive);

            if (!string.IsNullOrWhiteSpace(q))
                query = query.Where(x => x.FullName.Contains(q) || (x.VolunteerCode != null && x.VolunteerCode.Contains(q)) || (x.PhoneNumber != null && x.PhoneNumber.Contains(q)));

            if (!string.IsNullOrWhiteSpace(area))
                query = query.Where(x => x.PreferredArea != null && x.PreferredArea.Contains(area));

            if (skillDefinitionId.HasValue)
            {
                var volunteerIds = await _db.Set<VolunteerSkill>()
                    .Where(x => x.SkillDefinitionId == skillDefinitionId.Value)
                    .Select(x => x.VolunteerId)
                    .Distinct()
                    .ToListAsync();

                query = query.Where(x => volunteerIds.Contains(x.Id));
            }

            return await query.OrderBy(x => x.FullName).ToListAsync();
        }

        public async Task<Volunteer?> GetProfileAsync(Guid id)
            => await _db.Set<Volunteer>()
                .Include(x => x.Assignments)
                .Include(x => x.HourLogs)
                .FirstOrDefaultAsync(x => x.Id == id);
    }
}
