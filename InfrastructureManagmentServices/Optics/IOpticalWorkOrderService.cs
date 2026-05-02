using InfrastructureManagmentWebFramework.Models.Optics.WorkOrders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Optics
{
    public interface IOpticalWorkOrderService
    {
        Task<Guid> CreateAsync(CreateOpticalWorkOrderVm vm);
        Task MarkInLabAsync(Guid id);
        Task MarkReadyAsync(Guid id);
        Task MarkDeliveredAsync(Guid id);
    }
}
