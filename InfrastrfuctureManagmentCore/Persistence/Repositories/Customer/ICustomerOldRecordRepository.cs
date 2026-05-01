using InfrastrfuctureManagmentCore.Domains.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Customer
{
    public interface ICustomerOldRecordRepository
    {
        Task<List<CustomerOldRecord>> GetByCustomerIdAsync(Guid customerId);
        Task<int> GetCountByCustomerIdAsync(Guid customerId);
        Task<CustomerOldRecord?> GetByIdAsync(Guid id);

        Task AddAsync(CustomerOldRecord record);
        Task UpdateAsync(CustomerOldRecord record);
    }
}
