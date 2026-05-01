using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Expenses;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class ExpenseCategoryRepository : IExpenseCategoryRepository
    {
        private readonly AppDbContext _db;

        public ExpenseCategoryRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<ExpenseCategory>> GetAllAsync()
        {
            return await _db.ExpenseCategories
                .AsNoTracking()
                .OrderBy(x => x.CategoryCode)
                .ToListAsync();
        }

        public async Task<List<ExpenseCategory>> GetActiveAsync()
        {
            return await _db.ExpenseCategories
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.CategoryCode)
                .ToListAsync();
        }

        public async Task<ExpenseCategory?> GetByIdAsync(Guid id)
        {
            return await _db.ExpenseCategories
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _db.ExpenseCategories
                .AnyAsync(x => x.CategoryCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return await _db.ExpenseCategories
                .AnyAsync(x => x.CategoryCode == code && x.Id != excludeId);
        }

        public async Task AddAsync(ExpenseCategory category)
        {
            await _db.ExpenseCategories.AddAsync(category);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(ExpenseCategory category)
        {
            _db.ExpenseCategories.Update(category);
            await _db.SaveChangesAsync();
        }
    }
}
