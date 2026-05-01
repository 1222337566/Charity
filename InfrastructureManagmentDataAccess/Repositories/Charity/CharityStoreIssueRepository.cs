using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class CharityStoreIssueRepository : ICharityStoreIssueRepository
    {
        private readonly AppDbContext _db;
        public CharityStoreIssueRepository(AppDbContext db) => _db = db;

        public Task<List<CharityStoreIssue>> GetAllAsync()
            => _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Beneficiary)
                .Include(x => x.Project)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.IssueDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<CharityStoreIssue?> GetByIdAsync(Guid id)
            => _db.Set<CharityStoreIssue>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Beneficiary)
                .Include(x => x.Project)
                .Include(x => x.Lines)
                    .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> NumberExistsAsync(string issueNumber)
            => _db.Set<CharityStoreIssue>().AnyAsync(x => x.IssueNumber == issueNumber);

        public async Task AddAsync(CharityStoreIssue entity)
        {
            _db.Set<CharityStoreIssue>().Add(entity);
            await _db.SaveChangesAsync();
        }
    }
}
