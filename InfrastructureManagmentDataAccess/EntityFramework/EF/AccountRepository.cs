using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using global::InfrastrfuctureManagmentCore.Domains.Financial;
using global::InfrastrfuctureManagmentCore.Persistence.Repositories.Financial_Account;
using InfrastructureManagmentCore.Domains.Profiling;
using Microsoft.EntityFrameworkCore;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    

   

        public class AccountRepository : IAccountRepository
        {
            private readonly AppDbContext _db;

            public AccountRepository(AppDbContext db)
            {
                _db = db;
            }

            public async Task<List<FinancialAccount>> GetAllAsync()
            {
                return await _db.Accounts
                    .AsNoTracking()
                    .OrderBy(x => x.AccountCode)
                    .ToListAsync();
            }

            public async Task<FinancialAccount?> GetByIdAsync(Guid id)
            {
                return await _db.Accounts
                    .FirstOrDefaultAsync(x => x.Id == id);
            }

            public async Task<FinancialAccount?> GetByCodeAsync(string accountCode)
            {
                return await _db.Accounts
                    .FirstOrDefaultAsync(x => x.AccountCode == accountCode);
            }

            public async Task<List<FinancialAccount>> GetParentsAsync()
            {
                return await _db.Accounts
                    .AsNoTracking()
                    .Where(x => !x.IsPosting)
                    .OrderBy(x => x.AccountCode)
                    .ToListAsync();
            }

            public async Task AddAsync(FinancialAccount account)
            {
                await _db.Accounts.AddAsync(account);
                await _db.SaveChangesAsync();
            }

            public async Task UpdateAsync(FinancialAccount account)
            {
                _db.Accounts.Update(account);
                await _db.SaveChangesAsync();
            }


        public async Task<List<FinancialAccount>> GetAllParentsAsync()
        {
            return await _db.Accounts
                .AsNoTracking()
                .Where(x => !x.IsPosting)
                .OrderBy(x => x.AccountCode)
                .ToListAsync();
        }

        public async Task<bool> AccountCodeExistsAsync(string accountCode)
        {
            return await _db.Accounts.AnyAsync(x => x.AccountCode == accountCode);
        }

        public async Task<bool> HasChildrenAsync(Guid accountId)
        {
            return await _db.Accounts.AnyAsync(x => x.ParentAccountId == accountId);
        }

        public async Task<bool> AccountCodeExistsAsync(string accountCode, Guid excludeId)
        {
            return await _db.Accounts.AnyAsync(x => x.AccountCode == accountCode && x.Id != excludeId);
        }
        public async Task<bool> HasActiveChildrenAsync(Guid accountId)
        {
            return await _db.Accounts.AnyAsync(x => x.ParentAccountId == accountId && x.IsActive);
        }

        public async Task SaveChangesAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
    

}
