using InfrastructureManagmentWebFramework.Models.Purchase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Purchase
{
    public interface IPurchaseService
    {
        Task CreateAsync(CreatePurchaseInvoiceVm vm);
    }
}
