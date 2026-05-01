using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.StockAdvanced
{
    public class StockReturnVoucherRepository : IStockReturnVoucherRepository
    {
        private readonly AppDbContext _db;
        public StockReturnVoucherRepository(AppDbContext db) => _db = db;

        public async Task<List<StockReturnVoucher>> GetAllAsync() => await _db.Set<StockReturnVoucher>()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.VoucherDate)
            .ToListAsync();

        public async Task<StockReturnVoucher?> GetByIdAsync(Guid id) => await _db.Set<StockReturnVoucher>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(StockReturnVoucher entity)
        {
            _db.Set<StockReturnVoucher>().Add(entity);
            await _db.SaveChangesAsync();
        }
    }
}
