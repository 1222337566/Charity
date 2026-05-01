using InfrastructureManagmentWebFramework.Models.Sale;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Sales
{
    public interface ISalesService
    {
        Task<Guid> CreateAsync(CreateSalesInvoiceVm vm);
    }
}
