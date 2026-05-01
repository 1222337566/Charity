using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Domains.Sale;
using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Customers
{
    public class CustomerAccountService : ICustomerAccountService
    {
        private readonly ICustomerAccountTransactionRepository _customerAccountTransactionRepository;

        public CustomerAccountService(ICustomerAccountTransactionRepository customerAccountTransactionRepository)
        {
            _customerAccountTransactionRepository = customerAccountTransactionRepository;
        }
        public async Task PostReceiptAsync(CustomerReceipt receipt)
        {
            var tx = new CustomerAccountTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = receipt.CustomerId,
                TransactionDateUtc = receipt.ReceiptDateUtc,
                TransactionType = CustomerAccountTransactionType.Receipt,
                ReferenceType = "CustomerReceipt",
                ReferenceId = receipt.Id,
                ReferenceNumber = receipt.ReceiptNumber,
                Description = $"سند قبض رقم {receipt.ReceiptNumber}",
                DebitAmount = 0,
                CreditAmount = receipt.Amount,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _customerAccountTransactionRepository.AddAsync(tx);
        }
        public async Task PostSaleInvoiceAsync(SalesInvoice invoice)
        {
            if (!invoice.CustomerId.HasValue || invoice.CustomerId == Guid.Empty)
                return;

            var tx = new CustomerAccountTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = invoice.CustomerId.Value,
                TransactionDateUtc = invoice.InvoiceDateUtc,
                TransactionType = CustomerAccountTransactionType.SaleInvoice,
                ReferenceType = "SalesInvoice",
                ReferenceId = invoice.Id,
                ReferenceNumber = invoice.InvoiceNumber,
                Description = $"فاتورة بيع رقم {invoice.InvoiceNumber}",
                DebitAmount = invoice.NetAmount,
                CreditAmount = 0,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _customerAccountTransactionRepository.AddAsync(tx);
        }

        public async Task PostSalesReturnAsync(SalesReturnInvoice invoice)
        {
            if (invoice.OriginalSalesInvoice == null || !invoice.OriginalSalesInvoice.CustomerId.HasValue)
                return;

            var tx = new CustomerAccountTransaction
            {
                Id = Guid.NewGuid(),
                CustomerId = invoice.OriginalSalesInvoice.CustomerId.Value,
                TransactionDateUtc = invoice.ReturnDateUtc,
                TransactionType = CustomerAccountTransactionType.SalesReturn,
                ReferenceType = "SalesReturn",
                ReferenceId = invoice.Id,
                ReferenceNumber = invoice.ReturnNumber,
                Description = $"مرتجع مبيعات رقم {invoice.ReturnNumber}",
                DebitAmount = 0,
                CreditAmount = invoice.NetAmount,
                CreatedAtUtc = DateTime.UtcNow
            };

            await _customerAccountTransactionRepository.AddAsync(tx);
        }

        public async Task<decimal> GetBalanceAsync(Guid customerId)
        {
            return await _customerAccountTransactionRepository.GetBalanceByCustomerIdAsync(customerId);
        }
    }
}
