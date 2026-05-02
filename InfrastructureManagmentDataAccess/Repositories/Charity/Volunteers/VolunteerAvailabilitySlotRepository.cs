using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerAvailabilitySlotRepository : IVolunteerAvailabilitySlotRepository
    {
        private readonly AppDbContext _db;
        public VolunteerAvailabilitySlotRepository(AppDbContext db) => _db = db;

        public async Task<List<VolunteerAvailabilitySlot>> GetByVolunteerIdAsync(Guid volunteerId)
            => await _db.Set<VolunteerAvailabilitySlot>()
                .AsNoTracking()
                .Where(x => x.VolunteerId == volunteerId)
                .OrderBy(x => x.DayOfWeekName).ThenBy(x => x.FromTime)
                .ToListAsync();

        public async Task<VolunteerAvailabilitySlot?> GetByIdAsync(Guid id)
            => await _db.Set<VolunteerAvailabilitySlot>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(VolunteerAvailabilitySlot entity)
        {
            _db.Set<VolunteerAvailabilitySlot>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(VolunteerAvailabilitySlot entity)
        {
            _db.Set<VolunteerAvailabilitySlot>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
