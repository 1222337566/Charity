using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.SalesReturn;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.salesreturn;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class SalesReturnRepository : ISalesReturnRepository
    {
        private readonly AppDbContext _db;

        public SalesReturnRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SalesReturnInvoice>> GetAllAsync()
        {
            return await _db.SalesReturnInvoices
                .AsNoTracking()
                .Include(x => x.OriginalSalesInvoice)
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.ReturnDateUtc)
                .ToListAsync();
        }

        public async Task<SalesReturnInvoice?> GetByIdAsync(Guid id)
        {
            return await _db.SalesReturnInvoices
                .Include(x => x.OriginalSalesInvoice)
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ReturnNumberExistsAsync(string returnNumber)
        {
            return await _db.SalesReturnInvoices
                .AnyAsync(x => x.ReturnNumber == returnNumber);
        }

        public async Task<decimal> GetReturnedQtyForOriginalLineAsync(Guid originalSalesInvoiceLineId)
        {
            return await _db.SalesReturnLines
                .Where(x => x.OriginalSalesInvoiceLineId == originalSalesInvoiceLineId)
                .SumAsync(x => (decimal?)x.Quantity) ?? 0;
        }

        public async Task AddAsync(SalesReturnInvoice invoice)
        {
            await _db.SalesReturnInvoices.AddAsync(invoice);
            await _db.SaveChangesAsync();
        }
    }
}
