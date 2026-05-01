using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions
{
    public interface IPrescriptionRepository
    {
        Task<List<Prescription>> GetByCustomerIdAsync(Guid customerId);
        Task<int> GetCountByCustomerIdAsync(Guid customerId);
        Task<Prescription?> GetByIdAsync(Guid id);
        Task<List<Prescription>> GetActiveByCustomerIdAsync(Guid customerId);
        Task AddAsync(Prescription prescription);
        Task UpdateAsync(Prescription prescription);
    }
}
