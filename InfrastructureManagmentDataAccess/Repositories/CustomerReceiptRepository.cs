using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
namespace InfrastructureManagmentDataAccess.Repositories
{
 

    public class CustomerReceiptRepository : ICustomerReceiptRepository
    {
        private readonly AppDbContext _db;

        public CustomerReceiptRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CustomerReceipt>> GetAllAsync()
        {
            return await _db.CustomerReceipts
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.PaymentMethod)
                .Include(x => x.SalesInvoice)
                .OrderByDescending(x => x.ReceiptDateUtc)
                .ToListAsync();
        }

        public async Task<List<CustomerReceipt>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _db.CustomerReceipts
                .AsNoTracking()
                .Include(x => x.Customer)
                .Include(x => x.PaymentMethod)
                .Include(x => x.SalesInvoice)
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.ReceiptDateUtc)
                .ToListAsync();
        }

        public async Task<CustomerReceipt?> GetByIdAsync(Guid id)
        {
            return await _db.CustomerReceipts
                .Include(x => x.Customer)
                .Include(x => x.PaymentMethod)
                .Include(x => x.SalesInvoice)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ReceiptNumberExistsAsync(string receiptNumber)
        {
            return await _db.CustomerReceipts
                .AnyAsync(x => x.ReceiptNumber == receiptNumber);
        }

        public async Task AddAsync(CustomerReceipt receipt)
        {
            await _db.CustomerReceipts.AddAsync(receipt);
            await _db.SaveChangesAsync();
        }
    }
}
