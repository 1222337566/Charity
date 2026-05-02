using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerAvailabilitySlotRepository
    {
        Task<List<VolunteerAvailabilitySlot>> GetByVolunteerIdAsync(Guid volunteerId);
        Task<VolunteerAvailabilitySlot?> GetByIdAsync(Guid id);
        Task AddAsync(VolunteerAvailabilitySlot entity);
        Task UpdateAsync(VolunteerAvailabilitySlot entity);
    }
}
