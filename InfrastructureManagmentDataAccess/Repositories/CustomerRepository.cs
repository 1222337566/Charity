using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Customer;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
using InfrastructureManagmentCore.Domains.Profiling;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
namespace InfrastructureManagmentDataAccess.Repositories
{
  

    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _db;

        public CustomerRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CustomerClient>> GetAllAsync()
        {
            return await _db.Customers
                .AsNoTracking()
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }

        public async Task<List<CustomerClient>> GetActiveAsync()
        {
            return await _db.Customers
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }
        public async Task<List<CustomerClient>> SearchAsync(string? q, bool? isActive)
        {
            var query = _db.Customers.AsNoTracking().AsQueryable();

            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();

                query = query.Where(x =>
                    x.CustomerNumber.Contains(q) ||
                    x.NameAr.Contains(q) ||
                    (x.NameEn != null && x.NameEn.Contains(q)) ||
                    (x.MobileNo != null && x.MobileNo.Contains(q)) ||
                    (x.Tel != null && x.Tel.Contains(q)));
            }

            if (isActive.HasValue)
            {
                query = query.Where(x => x.IsActive == isActive.Value);
            }

            return await query
                .OrderByDescending(x => x.CreatedAtUtc)
                .ToListAsync();
        }
        public async Task<CustomerClient?> GetByIdAsync(Guid id)
        {
            return await _db.Customers
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<CustomerClient?> GetByNumberAsync(string customerNumber)
        {
            return await _db.Customers
                .FirstOrDefaultAsync(x => x.CustomerNumber == customerNumber);
        }

        public async Task<bool> NumberExistsAsync(string customerNumber)
        {
            return await _db.Customers
                .AnyAsync(x => x.CustomerNumber == customerNumber);
        }

        public async Task<bool> NumberExistsAsync(string customerNumber, Guid excludeId)
        {
            return await _db.Customers
                .AnyAsync(x => x.CustomerNumber == customerNumber && x.Id != excludeId);
        }

        public async Task AddAsync(CustomerClient customer)
        {
            await _db.Customers.AddAsync(customer);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomerClient customer)
        {
            _db.Customers.Update(customer);
            await _db.SaveChangesAsync();
        }
    }
}
