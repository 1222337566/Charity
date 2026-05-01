using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerHourLogRepository
    {
        Task<List<VolunteerHourLog>> GetByVolunteerIdAsync(Guid volunteerId);
        Task<VolunteerHourLog?> GetByIdAsync(Guid id);
        Task AddAsync(VolunteerHourLog entity);
    }
}
