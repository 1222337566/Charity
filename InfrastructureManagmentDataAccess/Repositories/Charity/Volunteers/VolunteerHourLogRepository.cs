using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerHourLogRepository : IVolunteerHourLogRepository
    {
        private readonly AppDbContext _db;
        public VolunteerHourLogRepository(AppDbContext db) => _db = db;

        public async Task<List<VolunteerHourLog>> GetByVolunteerIdAsync(Guid volunteerId)
            => await _db.Set<VolunteerHourLog>()
                .AsNoTracking()
                .Where(x => x.VolunteerId == volunteerId)
                .OrderByDescending(x => x.WorkDate)
                .ToListAsync();

        public async Task<VolunteerHourLog?> GetByIdAsync(Guid id)
            => await _db.Set<VolunteerHourLog>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(VolunteerHourLog entity)
        {
            _db.Set<VolunteerHourLog>().Add(entity);
            await _db.SaveChangesAsync();
        }
    }
}
