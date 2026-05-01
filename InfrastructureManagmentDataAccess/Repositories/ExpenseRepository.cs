using InfrastructureManagmentCore.Domains.Products;
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

    public class ExpenseRepository : IExpenseRepository
    {
        private readonly AppDbContext _db;

        public ExpenseRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Expensex>> GetAllAsync()
        {
            return await _db.Expenses
                .AsNoTracking()
                .Include(x => x.ExpenseCategory)
                .Include(x => x.PaymentMethod)
                .OrderByDescending(x => x.ExpenseDateUtc)
                .ToListAsync();
        }

        public async Task<Expensex?> GetByIdAsync(Guid id)
        {
            return await _db.Expenses
                .Include(x => x.ExpenseCategory)
                .Include(x => x.PaymentMethod)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<bool> ExpenseNumberExistsAsync(string expenseNumber)
        {
            return await _db.Expenses
                .AnyAsync(x => x.ExpenseNumber == expenseNumber);
        }

        public async Task AddAsync(Expensex expense)
        {
            await _db.Expenses.AddAsync(expense);
            await _db.SaveChangesAsync();
        }
    }
}
