using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerRepository : IVolunteerRepository
    {
        private readonly AppDbContext _db;
        public VolunteerRepository(AppDbContext db) => _db = db;

        public async Task<List<Volunteer>> GetAllAsync()
            => await _db.Set<Volunteer>()
                .AsNoTracking()
                .Include(x => x.Assignments)
                .Include(x => x.HourLogs)
                .OrderBy(x => x.FullName)
                .ToListAsync();

        public async Task<Volunteer?> GetByIdAsync(Guid id)
            => await _db.Set<Volunteer>()
                .Include(x => x.Assignments)
                .Include(x => x.HourLogs)
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(Volunteer entity)
        {
            _db.Set<Volunteer>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Volunteer entity)
        {
            _db.Set<Volunteer>().Update(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> VolunteerCodeExistsAsync(string volunteerCode, Guid? excludeId = null)
            => await _db.Set<Volunteer>()
                .AnyAsync(x => x.VolunteerCode == volunteerCode && (!excludeId.HasValue || x.Id != excludeId.Value));
    }
}
