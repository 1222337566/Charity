using InfrastructureManagmentDataAccess.EntityFramework;
using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.Volunteers
{
    public class VolunteerProjectAssignmentRepository : IVolunteerProjectAssignmentRepository
    {
        private readonly AppDbContext _db;
        public VolunteerProjectAssignmentRepository(AppDbContext db) => _db = db;

        public async Task<List<VolunteerProjectAssignment>> GetByVolunteerIdAsync(Guid volunteerId)
            => await _db.Set<VolunteerProjectAssignment>()
                .AsNoTracking()
                .Where(x => x.VolunteerId == volunteerId)
                .OrderByDescending(x => x.StartDate)
                .ToListAsync();

        public async Task<VolunteerProjectAssignment?> GetByIdAsync(Guid id)
            => await _db.Set<VolunteerProjectAssignment>().FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(VolunteerProjectAssignment entity)
        {
            _db.Set<VolunteerProjectAssignment>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(VolunteerProjectAssignment entity)
        {
            _db.Set<VolunteerProjectAssignment>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
