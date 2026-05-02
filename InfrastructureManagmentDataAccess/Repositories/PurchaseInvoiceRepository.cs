using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Purchase;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class PurchaseInvoiceRepository : IPurchaseInvoiceRepository
    {
        private readonly AppDbContext _db;

        public PurchaseInvoiceRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PurchaseInvoice>> GetAllAsync()
        {
            return await _db.PurchaseInvoices
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();
        }

        public async Task<PurchaseInvoice?> GetByIdAsync(Guid id)
        {
            return await _db.PurchaseInvoices
                .Include(x => x.Warehouse)
                .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<PurchaseInvoice>> GetBySupplierIdAsync(Guid supplierId)
        {
            return await _db.PurchaseInvoices
                .AsNoTracking()
                .Include(x => x.Warehouse)
                .Where(x => x.SupplierId == supplierId)
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();
        }
        public async Task<List<PurchaseInvoice>> SearchAsync(
    DateTime? dateFrom,
    DateTime? dateTo,
    Guid? supplierId,
    Guid? warehouseId,
    string? q)
        {
            var query = _db.PurchaseInvoices
                .AsNoTracking()
                .Include(x => x.Supplier)
                .Include(x => x.Warehouse)
                .AsQueryable();

            if (dateFrom.HasValue)
                query = query.Where(x => x.InvoiceDateUtc.Date >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(x => x.InvoiceDateUtc.Date <= dateTo.Value.Date);

            if (supplierId.HasValue && supplierId.Value != Guid.Empty)
                query = query.Where(x => x.SupplierId == supplierId.Value);

            if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
                query = query.Where(x => x.WarehouseId == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.InvoiceNumber.Contains(q) ||
                    x.SupplierName.Contains(q) ||
                    (x.SupplierInvoiceNumber != null && x.SupplierInvoiceNumber.Contains(q)));
            }

            return await query
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();
        }
        public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber)
        {
            return await _db.PurchaseInvoices
                .AnyAsync(x => x.InvoiceNumber == invoiceNumber);
        }

        public async Task AddAsync(PurchaseInvoice invoice)
        {
            await _db.PurchaseInvoices.AddAsync(invoice);
            await _db.SaveChangesAsync();
        }
    }
}
