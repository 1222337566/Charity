using InfrastrfuctureManagmentCore.Domains.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Purchase
{
    public interface IPurchaseInvoiceRepository
    {
        Task<List<PurchaseInvoice>> GetAllAsync();
        Task<PurchaseInvoice?> GetByIdAsync(Guid id);
        Task<bool> InvoiceNumberExistsAsync(string invoiceNumber);
        Task<List<PurchaseInvoice>> GetBySupplierIdAsync(Guid supplierId);
        Task AddAsync(PurchaseInvoice invoice);
        Task<List<PurchaseInvoice>> SearchAsync(
    DateTime? dateFrom,
    DateTime? dateTo,
    Guid? supplierId,
    Guid? warehouseId,
    string? q);
    }
}
