using InfrastrfuctureManagmentCore.Domains.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Customer
{
    public interface ICustomerAccountTransactionRepository
    {
        Task<List<CustomerAccountTransaction>> GetByCustomerIdAsync(Guid customerId);
        Task<decimal> GetBalanceByCustomerIdAsync(Guid customerId);
        Task AddAsync(CustomerAccountTransaction transaction);
    }
}
