using InfrastructureManagmentWebFramework.Models.Salesreturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Salesreturn
{
    public interface ISalesReturnService
    {
        Task<Guid> CreateAsync(CreateSalesReturnVm vm);
    }
}
