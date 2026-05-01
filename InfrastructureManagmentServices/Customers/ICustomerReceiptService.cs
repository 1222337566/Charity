using InfrastructureManagmentWebFramework.Models.Optics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Customers
{
    public interface ICustomerReceiptService
    {
        Task<Guid> CreateAsync(CreateCustomerReceiptVm vm);
    }
}
