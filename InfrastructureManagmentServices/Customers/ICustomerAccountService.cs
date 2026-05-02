using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Customers
{
  
        public interface ICustomerAccountService
        {
            Task PostSaleInvoiceAsync(SalesInvoice invoice);
            Task PostSalesReturnAsync(SalesReturnInvoice invoice);
            public Task PostReceiptAsync(CustomerReceipt receipt);
            Task<decimal> GetBalanceAsync(Guid customerId);
        }
    
}
