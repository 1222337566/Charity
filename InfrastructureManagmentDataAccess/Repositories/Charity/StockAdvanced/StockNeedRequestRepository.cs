using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.StockAdvanced
{
    public class StockNeedRequestRepository : IStockNeedRequestRepository
    {
        private readonly AppDbContext _db;
        public StockNeedRequestRepository(AppDbContext db) => _db = db;

        public async Task<List<StockNeedRequest>> GetAllAsync() => await _db.Set<StockNeedRequest>()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.RequestDate)
            .ToListAsync();

        public async Task<StockNeedRequest?> GetByIdAsync(Guid id) => await _db.Set<StockNeedRequest>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(StockNeedRequest entity)
        {
            _db.Set<StockNeedRequest>().Add(entity);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(StockNeedRequest entity)
        {
            _db.Set<StockNeedRequest>().Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}
