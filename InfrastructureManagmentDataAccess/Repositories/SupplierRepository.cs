using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Suppliers;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Suppliers;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class SupplierRepository : ISupplierRepository
    {
        private readonly AppDbContext _db;

        public SupplierRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Supplier>> GetAllAsync()
        {
            return await _db.Suppliers
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<Supplier>> GetActiveAsync()
        {
            return await _db.Suppliers
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(Guid id)
        {
            return await _db.Suppliers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Supplier?> GetByNumberAsync(string supplierNumber)
        {
            return await _db.Suppliers
                .FirstOrDefaultAsync(x => x.SupplierNumber == supplierNumber);
        }

        public async Task<bool> NumberExistsAsync(string supplierNumber)
        {
            return await _db.Suppliers
                .AnyAsync(x => x.SupplierNumber == supplierNumber);
        }

        public async Task<bool> NumberExistsAsync(string supplierNumber, Guid excludeId)
        {
            return await _db.Suppliers
                .AnyAsync(x => x.SupplierNumber == supplierNumber && x.Id != excludeId);
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _db.Suppliers.AddAsync(supplier);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _db.Suppliers.Update(supplier);
            await _db.SaveChangesAsync();
        }
    }
}
