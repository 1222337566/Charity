using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.salesreturn
{
    public interface ISalesReturnRepository
    {
        Task<List<SalesReturnInvoice>> GetAllAsync();
        Task<SalesReturnInvoice?> GetByIdAsync(Guid id);
        Task<bool> ReturnNumberExistsAsync(string returnNumber);
        Task<decimal> GetReturnedQtyForOriginalLineAsync(Guid originalSalesInvoiceLineId);
        Task AddAsync(SalesReturnInvoice invoice);
    }
}
