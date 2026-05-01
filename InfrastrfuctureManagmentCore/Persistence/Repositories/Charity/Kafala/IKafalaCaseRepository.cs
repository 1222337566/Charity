using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala
{
    public interface IKafalaCaseRepository
    {
        Task<List<KafalaCase>> GetAllAsync(Guid? sponsorId = null);
        Task<List<KafalaCase>> GetActiveDueCasesAsync(DateTime dueBeforeUtc);
        Task<KafalaCase?> GetByIdAsync(Guid id);
        Task<KafalaCase?> GetByIdWithDetailsAsync(Guid id);
        Task<bool> CaseNumberExistsAsync(string caseNumber, Guid? ignoreId = null);
        Task<KafalaCase> AddAsync(KafalaCase entity);
        Task UpdateAsync(KafalaCase entity);
    }
}
