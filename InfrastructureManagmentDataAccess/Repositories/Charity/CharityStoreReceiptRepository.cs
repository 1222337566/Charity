using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.Stores;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity
{
    public class CharityStoreReceiptRepository : ICharityStoreReceiptRepository
    {
        private readonly AppDbContext _db;
        public CharityStoreReceiptRepository(AppDbContext db) => _db = db;

        public Task<List<CharityStoreReceipt>> GetAllAsync()
            => _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.ReceiptDate)
                .ThenByDescending(x => x.CreatedAtUtc)
                .ToListAsync();

        public Task<CharityStoreReceipt?> GetByIdAsync(Guid id)
            => _db.Set<CharityStoreReceipt>()
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                    .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);

        public Task<bool> NumberExistsAsync(string receiptNumber)
            => _db.Set<CharityStoreReceipt>().AnyAsync(x => x.ReceiptNumber == receiptNumber);

        public async Task AddAsync(CharityStoreReceipt entity)
        {
            _db.Set<CharityStoreReceipt>().Add(entity);
            await _db.SaveChangesAsync();
        }
    }
}
