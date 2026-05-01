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
   

    public class CustomerAccountTransactionRepository : ICustomerAccountTransactionRepository
    {
        private readonly AppDbContext _db;

        public CustomerAccountTransactionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CustomerAccountTransaction>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _db.CustomerAccountTransactions
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderBy(x => x.TransactionDateUtc)
                .ThenBy(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<decimal> GetBalanceByCustomerIdAsync(Guid customerId)
        {
            var debit = await _db.CustomerAccountTransactions
                .Where(x => x.CustomerId == customerId)
                .SumAsync(x => (decimal?)x.DebitAmount) ?? 0;

            var credit = await _db.CustomerAccountTransactions
                .Where(x => x.CustomerId == customerId)
                .SumAsync(x => (decimal?)x.CreditAmount) ?? 0;

            return debit - credit;
        }

        public async Task AddAsync(CustomerAccountTransaction transaction)
        {
            await _db.CustomerAccountTransactions.AddAsync(transaction);
            await _db.SaveChangesAsync();
        }
    }
}
