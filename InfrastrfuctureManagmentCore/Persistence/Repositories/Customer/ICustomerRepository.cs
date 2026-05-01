using InfrastrfuctureManagmentCore.Domains.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Customer
{
    public interface ICustomerRepository
    {
        Task<List<CustomerClient>> GetAllAsync();
        Task<List<CustomerClient>> SearchAsync(string? q, bool? isActive);
        Task<List<CustomerClient>> GetActiveAsync();
        Task<CustomerClient?> GetByIdAsync(Guid id);
        Task<CustomerClient?> GetByNumberAsync(string customerNumber);

        Task<bool> NumberExistsAsync(string customerNumber);
        Task<bool> NumberExistsAsync(string customerNumber, Guid excludeId);

        Task AddAsync(CustomerClient customer);
        Task UpdateAsync(CustomerClient customer);
    }
}
