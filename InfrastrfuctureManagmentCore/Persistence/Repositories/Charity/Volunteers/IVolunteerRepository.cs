using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerRepository
    {
        Task<List<Volunteer>> GetAllAsync();
        Task<Volunteer?> GetByIdAsync(Guid id);
        Task AddAsync(Volunteer entity);
        Task UpdateAsync(Volunteer entity);
        Task<bool> VolunteerCodeExistsAsync(string volunteerCode, Guid? excludeId = null);
    }
}
