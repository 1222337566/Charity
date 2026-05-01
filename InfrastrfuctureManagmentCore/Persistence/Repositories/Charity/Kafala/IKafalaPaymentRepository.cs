using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala
{
    public interface IKafalaPaymentRepository
    {
        Task<List<KafalaPayment>> GetAllAsync(Guid? kafalaCaseId = null, Guid? sponsorId = null, Guid? aidCycleId = null);
        Task<KafalaPayment?> GetByIdAsync(Guid id);
        Task AddAsync(KafalaPayment entity);
        Task UpdateAsync(KafalaPayment entity);
    }
}
