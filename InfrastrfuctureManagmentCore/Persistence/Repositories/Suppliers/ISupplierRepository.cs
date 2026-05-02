using InfrastrfuctureManagmentCore.Domains.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers
{
    public interface ISupplierRepository
    {
        Task<List<Supplier>> GetAllAsync();
        Task<List<Supplier>> GetActiveAsync();
        Task<Supplier?> GetByIdAsync(Guid id);
        Task<Supplier?> GetByNumberAsync(string supplierNumber);

        Task<bool> NumberExistsAsync(string supplierNumber);
        Task<bool> NumberExistsAsync(string supplierNumber, Guid excludeId);

        Task AddAsync(Supplier supplier);
        Task UpdateAsync(Supplier supplier);
    }
}
