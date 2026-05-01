using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface ICharityProjectRepository
    {
        Task<List<CharityProject>> SearchAsync(string? q, string? status, bool? isActive);
        Task<CharityProject?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);
        Task AddAsync(CharityProject entity);
        Task UpdateAsync(CharityProject entity);
    }
}
