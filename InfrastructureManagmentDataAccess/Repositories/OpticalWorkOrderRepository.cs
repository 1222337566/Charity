using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Optics;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Optics;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class OpticalWorkOrderRepository : IOpticalWorkOrderRepository
    {
        private readonly AppDbContext _db;

        public OpticalWorkOrderRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<OpticalWorkOrder>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _db.OpticalWorkOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .Include(x => x.Prescription)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.OrderDateUtc)
                .ToListAsync();
        }
        public async Task<List<OpticalWorkOrder>> GetBySalesInvoiceIdsAsync(List<Guid> salesInvoiceIds)
        {
            if (salesInvoiceIds == null || !salesInvoiceIds.Any())
                return new List<OpticalWorkOrder>();

            return await _db.OpticalWorkOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .Where(x => salesInvoiceIds.Contains(x.SalesInvoiceId))
                .ToListAsync();
        }
        public async Task<List<OpticalWorkOrder>> GetByStatusAsync(OpticalWorkOrderStatus status)
        {
            return await _db.OpticalWorkOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .Include(x => x.Prescription)
                .Where(x => x.Status == status)
                .OrderByDescending(x => x.OrderDateUtc)
                .ToListAsync();
        }
        public async Task<List<OpticalWorkOrder>> GetReadyForDeliveryAsync()
        {
            return await _db.OpticalWorkOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .Include(x => x.Prescription)
                .Where(x => x.Status == OpticalWorkOrderStatus.Ready)
                .OrderByDescending(x => x.ReadyDateUtc)
                .ThenBy(x => x.ExpectedDeliveryDateUtc)
                .ToListAsync();
        }
        public async Task<int> GetDeliveredTodayCountAsync()
        {
            var today = DateTime.UtcNow.Date;

            return await _db.OpticalWorkOrders
                .CountAsync(x =>
                    x.Status == OpticalWorkOrderStatus.Delivered &&
                    x.DeliveredDateUtc.HasValue &&
                    x.DeliveredDateUtc.Value.Date == today);
        }

        public async Task<int> GetOverdueCountAsync()
        {
            var now = DateTime.UtcNow;

            return await _db.OpticalWorkOrders
                .CountAsync(x =>
                    x.Status != OpticalWorkOrderStatus.Delivered &&
                    x.Status != OpticalWorkOrderStatus.Cancelled &&
                    x.ExpectedDeliveryDateUtc.HasValue &&
                    x.ExpectedDeliveryDateUtc.Value < now);
        }
        public async Task<List<OpticalWorkOrder>> GetAllAsync()
        {
            return await _db.OpticalWorkOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .Include(x => x.Prescription)
                .OrderByDescending(x => x.OrderDateUtc)
                .ToListAsync();
        }

        public async Task<List<OpticalWorkOrder>> GetOpenAsync()
        {
            return await _db.OpticalWorkOrders
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .Where(x => x.Status != OpticalWorkOrderStatus.Delivered && x.Status != OpticalWorkOrderStatus.Cancelled)
                .OrderByDescending(x => x.OrderDateUtc)
                .ToListAsync();
        }

        public async Task<OpticalWorkOrder?> GetByIdAsync(Guid id)
        {
            return await _db.OpticalWorkOrders
                .Include(x => x.Customer)
                .Include(x => x.SalesInvoice)
                .ThenInclude(x => x.Lines)
                .ThenInclude(x => x.Item)
                .Include(x => x.Prescription)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OpticalWorkOrder?> GetBySalesInvoiceIdAsync(Guid salesInvoiceId)
        {
            return await _db.OpticalWorkOrders
                .FirstOrDefaultAsync(x => x.SalesInvoiceId == salesInvoiceId);
        }

        public async Task<bool> WorkOrderNumberExistsAsync(string workOrderNumber)
        {
            return await _db.OpticalWorkOrders
                .AnyAsync(x => x.WorkOrderNumber == workOrderNumber);
        }

        public async Task AddAsync(OpticalWorkOrder workOrder)
        {
            await _db.OpticalWorkOrders.AddAsync(workOrder);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(OpticalWorkOrder workOrder)
        {
            _db.OpticalWorkOrders.Update(workOrder);
            await _db.SaveChangesAsync();
        }
    }
}
