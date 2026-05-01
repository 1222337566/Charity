using InfrastrfuctureManagmentCore.Domains.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Customer
{
    public interface ICustomerReceiptRepository
    {
        Task<List<CustomerReceipt>> GetAllAsync();
        Task<List<CustomerReceipt>> GetByCustomerIdAsync(Guid customerId);
        Task<CustomerReceipt?> GetByIdAsync(Guid id);
        Task<bool> ReceiptNumberExistsAsync(string receiptNumber);
        Task AddAsync(CustomerReceipt receipt);
    }
}
