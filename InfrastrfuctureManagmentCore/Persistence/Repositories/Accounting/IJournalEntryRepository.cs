using InfrastrfuctureManagmentCore.Domains.Accounting;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Accounting
{
    public interface IJournalEntryRepository
    {
        Task<List<JournalEntry>> GetAllAsync();
        Task<JournalEntry?> GetByIdAsync(Guid id);
        Task<JournalEntryLine?> GetLineByIdAsync(Guid lineId);
        Task<bool> EntryNumberExistsAsync(string entryNumber);
        Task<bool> EntryNumberExistsAsync(string entryNumber, Guid excludeId);
        Task AddAsync(JournalEntry entity);
        Task UpdateAsync(JournalEntry entity);
        Task AddLineAsync(JournalEntryLine entity);
        Task UpdateLineAsync(JournalEntryLine entity);
        Task DeleteLineAsync(JournalEntryLine entity);
        Task SaveChangesAsync();
    }
}
