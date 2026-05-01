using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Beneficiaries
{
    public interface IBeneficiaryRepository
    {
        Task<List<Beneficiary>> SearchAsync(string? q, Guid? statusId, bool? isActive);
        Task<Beneficiary?> GetByIdAsync(Guid id);
        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);
        Task<bool> NationalIdExistsAsync(string nationalId);
        Task<bool> NationalIdExistsAsync(string nationalId, Guid excludeId);
        Task AddAsync(Beneficiary beneficiary);
        Task UpdateAsync(Beneficiary beneficiary);
    }
}
