using InfrastrfuctureManagmentCore.Domains.Charity.Donors;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Donors
{
    public interface IDonorRepository
    {
        Task<List<Donor>> SearchAsync(string? q, string? donorType, bool? isActive);
        Task<Donor?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);
        Task<bool> NationalIdOrTaxNoExistsAsync(string value);
        Task<bool> NationalIdOrTaxNoExistsAsync(string value, Guid excludeId);
        Task AddAsync(Donor donor);
        Task UpdateAsync(Donor donor);
    }
}
