using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Kafala
{
    public interface IKafalaSponsorRepository
    {
        Task<List<KafalaSponsor>> GetAllAsync();
        Task<KafalaSponsor?> GetByIdAsync(Guid id);
        Task<bool> SponsorCodeExistsAsync(string sponsorCode, Guid? ignoreId = null);
        Task AddAsync(KafalaSponsor entity);
        Task UpdateAsync(KafalaSponsor entity);
    }
}
