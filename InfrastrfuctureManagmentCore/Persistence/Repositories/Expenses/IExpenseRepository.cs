using InfrastrfuctureManagmentCore.Domains.Expenses;
using InfrastructureManagmentCore.Domains.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Expenses
{
    public interface IExpenseRepository
    {
        Task<List<Expensex>> GetAllAsync();
        Task<Expensex?> GetByIdAsync(Guid id);
        Task<bool> ExpenseNumberExistsAsync(string expenseNumber);
        Task AddAsync(Expensex expense);
    }
}
