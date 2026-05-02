using InfrastrfuctureManagmentCore.Domains.Charity.Funders;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Funders
{
    public interface IFunderRepository
    {
        Task<List<Funder>> SearchAsync(string? q, string? funderType, bool? isActive);
        Task<Funder?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code, Guid? excludeId = null);
        Task AddAsync(Funder funder);
        Task UpdateAsync(Funder funder);
    }
}
