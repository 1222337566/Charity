using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerProjectAssignmentRepository
    {
        Task<List<VolunteerProjectAssignment>> GetByVolunteerIdAsync(Guid volunteerId);
        Task<VolunteerProjectAssignment?> GetByIdAsync(Guid id);
        Task AddAsync(VolunteerProjectAssignment entity);
        Task UpdateAsync(VolunteerProjectAssignment entity);
    }
}
