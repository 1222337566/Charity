using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.AidCycles
{
    public interface IAidCycleRepository
    {
        Task<List<AidCycle>> GetAllAsync();
        Task<AidCycle?> GetByIdAsync(Guid id);
        Task<AidCycle?> GetByIdWithBeneficiariesAsync(Guid id);
        Task<bool> CycleNumberExistsAsync(string cycleNumber, Guid? excludeId = null);
        Task AddAsync(AidCycle entity);
        Task UpdateAsync(AidCycle entity);
        Task UpdateTotalsAsync(Guid cycleId);
    }
}
