using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerSkillRepository
    {
        Task<List<VolunteerSkill>> GetByVolunteerIdAsync(Guid volunteerId);
        Task<VolunteerSkill?> GetByIdAsync(Guid id);
        Task AddAsync(VolunteerSkill entity);
        Task UpdateAsync(VolunteerSkill entity);
    }
}
