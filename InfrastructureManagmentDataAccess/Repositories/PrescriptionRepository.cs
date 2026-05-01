using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Prescriptions;
using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
namespace InfrastructureManagmentDataAccess.Repositories
{
 

    public class PrescriptionRepository : IPrescriptionRepository
    {
        private readonly AppDbContext _db;

        public PrescriptionRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<Prescription>> GetByCustomerIdAsync(Guid customerId)
        {
            return await _db.Prescriptions
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId)
                .OrderByDescending(x => x.PrescriptionDateUtc)
                .ToListAsync();
        }

        public async Task<int> GetCountByCustomerIdAsync(Guid customerId)
        {
            return await _db.Prescriptions
                .CountAsync(x => x.CustomerId == customerId);
        }

        public async Task<Prescription?> GetByIdAsync(Guid id)
        {
            return await _db.Prescriptions
                .FirstOrDefaultAsync(x => x.Id == id);
        }
        public async Task<List<Prescription>> GetActiveByCustomerIdAsync(Guid customerId)
        {
            return await _db.Prescriptions
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId && x.IsActive)
                .OrderByDescending(x => x.PrescriptionDateUtc)
                .ToListAsync();
        }
        public async Task AddAsync(Prescription prescription)
        {
            await _db.Prescriptions.AddAsync(prescription);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(Prescription prescription)
        {
            _db.Prescriptions.Update(prescription);
            await _db.SaveChangesAsync();
        }
    }
}
