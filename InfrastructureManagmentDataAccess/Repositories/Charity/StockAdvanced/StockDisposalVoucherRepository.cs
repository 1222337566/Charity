using InfrastrfuctureManagmentCore.Domains.Charity.StockAdvanced;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Charity.StockAdvanced;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.Repositories.Charity.StockAdvanced
{
    public class StockDisposalVoucherRepository : IStockDisposalVoucherRepository
    {
        private readonly AppDbContext _db;
        public StockDisposalVoucherRepository(AppDbContext db) => _db = db;

        public async Task<List<StockDisposalVoucher>> GetAllAsync() => await _db.Set<StockDisposalVoucher>()
            .Include(x => x.Lines)
            .OrderByDescending(x => x.VoucherDate)
            .ToListAsync();

        public async Task<StockDisposalVoucher?> GetByIdAsync(Guid id) => await _db.Set<StockDisposalVoucher>()
            .Include(x => x.Lines)
            .FirstOrDefaultAsync(x => x.Id == id);

        public async Task AddAsync(StockDisposalVoucher entity)
        {
            _db.Set<StockDisposalVoucher>().Add(entity);
            await _db.SaveChangesAsync();
        }
    }
}
