using InfrastrfuctureManagmentCore.Domains.Expenses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses
{
    public interface IExpenseCategoryRepository
    {
        Task<List<ExpenseCategory>> GetAllAsync();
        Task<List<ExpenseCategory>> GetActiveAsync();
        Task<ExpenseCategory?> GetByIdAsync(Guid id);

        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);

        Task AddAsync(ExpenseCategory category);
        Task UpdateAsync(ExpenseCategory category);
    }
}
