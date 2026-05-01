using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerSkillDefinitionRepository
    {
        Task<List<VolunteerSkillDefinition>> GetAllAsync();
        Task<VolunteerSkillDefinition?> GetByIdAsync(Guid id);
        Task AddAsync(VolunteerSkillDefinition entity);
        Task UpdateAsync(VolunteerSkillDefinition entity);
    }
}
