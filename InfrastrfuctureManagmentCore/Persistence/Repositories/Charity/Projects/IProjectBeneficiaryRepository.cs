using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Projects
{
    public interface IProjectBeneficiaryRepository
    {
        Task<List<ProjectBeneficiary>> GetByProjectIdAsync(Guid projectId);
        Task<ProjectBeneficiary?> GetByIdAsync(Guid id);
        Task<bool> ExistsAsync(Guid projectId, Guid beneficiaryId);
        Task AddAsync(ProjectBeneficiary entity);
        Task UpdateAsync(ProjectBeneficiary entity);
    }
}
