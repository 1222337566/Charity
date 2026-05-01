using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerSkillDefinitionRepository : IVolunteerSkillDefinitionRepository
    {
        private readonly AppDbContext _db;
        public VolunteerSkillDefinitionRepository(AppDbContext db) => _db = db;

        public async Task<List<VolunteerSkillDefinition>> GetAllAsync()
            => await _db.Set<VolunteerSkillDefinition>()
                .AsNoTracking()
                .OrderBy(x => x.SortOrder).ThenBy(x => x.Name)
                .ToListAsync();

        public async Task<VolunteerSkillDefinition?> GetByIdAsync(Guid id)
            => await _db.Set<VolunteerSkillDefinition>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(VolunteerSkillDefinition entity)
        {
            _db.Set<VolunteerSkillDefinition>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(VolunteerSkillDefinition entity)
        {
            _db.Set<VolunteerSkillDefinition>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
