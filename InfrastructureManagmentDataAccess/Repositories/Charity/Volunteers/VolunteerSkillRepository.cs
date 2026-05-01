using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerSkillRepository : IVolunteerSkillRepository
    {
        private readonly AppDbContext _db;
        public VolunteerSkillRepository(AppDbContext db) => _db = db;

        public async Task<List<VolunteerSkill>> GetByVolunteerIdAsync(Guid volunteerId)
            => await _db.Set<VolunteerSkill>()
                .AsNoTracking()
                .Include(x => x.SkillDefinition)
                .Where(x => x.VolunteerId == volunteerId)
                .OrderBy(x => x.SkillDefinition!.SortOrder).ThenBy(x => x.SkillDefinition!.Name)
                .ToListAsync();

        public async Task<VolunteerSkill?> GetByIdAsync(Guid id)
            => await _db.Set<VolunteerSkill>()
                .Include(x => x.SkillDefinition)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(VolunteerSkill entity)
        {
            _db.Set<VolunteerSkill>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(VolunteerSkill entity)
        {
            _db.Set<VolunteerSkill>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
