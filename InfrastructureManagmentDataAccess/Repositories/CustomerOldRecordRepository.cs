using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Customer;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Customer;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class CustomerOldRecordRepository : ICustomerOldRecordRepository
    {
        private readonly AppDbContext _db;

        public CustomerOldRecordRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<CustomerOldRecord>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _db.CustomerOldRecords
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.RecordDateUtc)
                .ToListAsync();
        }

        public async Task<int> GetCountByCustomerIdAsync(Guid customerId)
        {
            return await _db.CustomerOldRecords
                .CountAsync(x => x.CustomerId == customerId);
        }

        public async Task<CustomerOldRecord?> GetByIdAsync(Guid id)
        {
            return await _db.CustomerOldRecords
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task AddAsync(CustomerOldRecord record)
        {
            await _db.CustomerOldRecords.AddAsync(record);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(CustomerOldRecord record)
        {
            _db.CustomerOldRecords.Update(record);
            await _db.SaveChangesAsync();
        }
    }
}
