using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastructureManagmentWebFramework.Models.Charity.AidCycles;
using System.Security.Claims;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles
{
    public interface IAidCycleBeneficiaryRepository
    {
        Task<List<AidCycleBeneficiary>> GetByCycleIdAsync(Guid cycleId);
        Task<AidCycleBeneficiary?> GetByIdAsync(Guid id);
        Task AddRangeAsync(IEnumerable<AidCycleBeneficiary> entities);
        Task UpdateAsync(AidCycleBeneficiary entity);
        Task<List<AidCycleBeneficiary>> GetDueListAsync(DateTime? dueDate = null);

        Task<IReadOnlyList<AidCycleEligibleBeneficiaryVm>> GetEligibleBeneficiariesForAddAsync(Guid aidCycleId);
        Task<int> AddBeneficiariesAsync(Guid aidCycleId, IEnumerable<Guid> beneficiaryIds, decimal? approvedAmount, string? notes, ClaimsPrincipal user);
    }
}

