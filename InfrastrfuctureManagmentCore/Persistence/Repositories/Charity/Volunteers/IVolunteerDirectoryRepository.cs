using InfrastrfuctureManagmentCore.Domains.Charity.Volunteers;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Volunteers
{
    public interface IVolunteerDirectoryRepository
    {
        Task<List<Volunteer>> SearchAsync(string? q, Guid? skillDefinitionId, string? area, bool activeOnly = true);
        Task<Volunteer?> GetProfileAsync(Guid id);
    }
}
