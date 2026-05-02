using InfrastrfuctureManagmentCore.Domains.Optics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Optics
{
    public interface IOpticalWorkOrderRepository
    {
        Task<List<OpticalWorkOrder>> GetAllAsync();
        Task<List<OpticalWorkOrder>> GetReadyForDeliveryAsync();
        Task<List<OpticalWorkOrder>> GetByStatusAsync(OpticalWorkOrderStatus status);
        Task<int> GetDeliveredTodayCountAsync();
        Task<int> GetOverdueCountAsync();
        Task<List<OpticalWorkOrder>> GetOpenAsync();
        Task<OpticalWorkOrder?> GetByIdAsync(Guid id);
        Task<List<OpticalWorkOrder>> GetByCustomerIdAsync(Guid customerId);
        Task<OpticalWorkOrder?> GetBySalesInvoiceIdAsync(Guid salesInvoiceId);
        Task<bool> WorkOrderNumberExistsAsync(string workOrderNumber);
        Task<List<OpticalWorkOrder>> GetBySalesInvoiceIdsAsync(List<Guid> salesInvoiceIds);
        Task AddAsync(OpticalWorkOrder workOrder);
        Task UpdateAsync(OpticalWorkOrder workOrder);
    }
}
