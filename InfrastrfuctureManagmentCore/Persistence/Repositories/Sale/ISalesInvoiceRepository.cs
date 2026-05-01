using InfrastrfuctureManagmentCore.Domains.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Sale
{
    public interface ISalesInvoiceRepository
    {
        Task<List<SalesInvoice>> GetAllAsync();
        Task<SalesInvoice?> GetByIdAsync(Guid id);
        Task<bool> InvoiceNumberExistsAsync(string invoiceNumber);
        Task AddAsync(SalesInvoice invoice);
        Task<List<SalesInvoiceLine>> GetSoldItemsByCustomerIdAsync(Guid customerId);
        Task<int> GetSoldItemsCountByCustomerIdAsync(Guid customerId);
        Task<List<SalesInvoice>> GetForSalesReportAsync();
        Task<List<SalesInvoice>> SearchAsync(
    DateTime? dateFrom,
    DateTime? dateTo,
    Guid? customerId,
    Guid? warehouseId,
    string? q,
    bool onlyWithPrescription);
    }
}
