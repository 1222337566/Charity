using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Sale;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Sale;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class SalesInvoiceRepository : ISalesInvoiceRepository
    {
        private readonly AppDbContext _db;

        public SalesInvoiceRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<SalesInvoice>> GetAllAsync()
        {
            return await _db.SalesInvoices
    .AsNoTracking()
    .Include(x => x.Warehouse)
    .Include(x => x.Customer)
    .Include(x => x.Prescription)
    .Include(x => x.Lines)
    .Include(x => x.Payments)
    .ThenInclude(x => x.PaymentMethod)
    .OrderByDescending(x => x.InvoiceDateUtc)
    .ToListAsync();
            
        }

        public async Task<SalesInvoice?> GetByIdAsync(Guid id)
        {
            return await _db.SalesInvoices
     .Include(x => x.Warehouse)
     .Include(x => x.Customer)
     .Include(x => x.Prescription)
     .Include(x => x.Lines)
     .ThenInclude(x => x.Item)
     .Include(x => x.Payments)
     .ThenInclude(x => x.PaymentMethod)
     .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> InvoiceNumberExistsAsync(string invoiceNumber)
        {
            return await _db.SalesInvoices
                .AnyAsync(x => x.InvoiceNumber == invoiceNumber);
        }

        public async Task AddAsync(SalesInvoice invoice)
        {
            await _db.SalesInvoices.AddAsync(invoice);
            await _db.SaveChangesAsync();
        }
        public async Task<List<SalesInvoiceLine>> GetSoldItemsByCustomerIdAsync(Guid customerId)
        {
            return await _db.SalesInvoiceLines
                .AsNoTracking()
                .Include(x => x.Item)
                .Include(x => x.SalesInvoice)
                .Where(x => x.SalesInvoice != null && x.SalesInvoice.CustomerId == customerId)
                .OrderByDescending(x => x.SalesInvoice!.InvoiceDateUtc)
                .ToListAsync();
        }
        public async Task<List<SalesInvoice>> GetForSalesReportAsync()
        {
            return await _db.SalesInvoices
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Warehouse)
                .Include(x => x.Prescription)
                .Include(x => x.Payments)
                .ThenInclude(x => x.PaymentMethod)
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();
        }
        public async Task<List<SalesInvoice>> SearchAsync(
    DateTime? dateFrom,
    DateTime? dateTo,
    Guid? customerId,
    Guid? warehouseId,
    string? q,
    bool onlyWithPrescription)
        {
            var query = _db.SalesInvoices
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.Warehouse)
                .Include(x => x.Prescription)
                .Include(x => x.Payments)
                .ThenInclude(x => x.PaymentMethod)
                .AsQueryable();

            if (dateFrom.HasValue)
                query = query.Where(x => x.InvoiceDateUtc.Date >= dateFrom.Value.Date);

            if (dateTo.HasValue)
                query = query.Where(x => x.InvoiceDateUtc.Date <= dateTo.Value.Date);

            if (customerId.HasValue && customerId.Value != Guid.Empty)
                query = query.Where(x => x.CustomerId == customerId.Value);

            if (warehouseId.HasValue && warehouseId.Value != Guid.Empty)
                query = query.Where(x => x.WarehouseId == warehouseId.Value);

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                query = query.Where(x =>
                    x.InvoiceNumber.Contains(q) ||
                    x.CustomerName.Contains(q));
            }

            if (onlyWithPrescription)
                query = query.Where(x => x.PrescriptionId.HasValue);

            return await query
                .OrderByDescending(x => x.InvoiceDateUtc)
                .ToListAsync();
        }
        public async Task<int> GetSoldItemsCountByCustomerIdAsync(Guid customerId)
        {
            return await _db.SalesInvoiceLines
                .CountAsync(x => x.SalesInvoice != null && x.SalesInvoice.CustomerId == customerId);
        }
    }
}
