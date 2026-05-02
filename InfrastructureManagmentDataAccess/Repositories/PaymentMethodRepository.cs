using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.Repositories
{
    using InfrastrfuctureManagmentCore.Domains.Payments;
    using InfrastrfuctureManagmentCore.Persistence.Repositories.Payments;
    using InfrastructureManagmentDataAccess.EntityFramework;
    using Microsoft.EntityFrameworkCore;

    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly AppDbContext _db;

        public PaymentMethodRepository(AppDbContext db)
        {
            _db = db;
        }

        public async Task<List<PaymentMethod>> GetAllAsync()
        {
            return await _db.PaymentMethods
                .AsNoTracking()
                .OrderBy(x => x.MethodCode)
                .ToListAsync();
        }

        public async Task<List<PaymentMethod>> GetActiveAsync()
        {
            return await _db.PaymentMethods
                .AsNoTracking()
                .Where(x => x.IsActive)
                .OrderBy(x => x.MethodCode)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(Guid id)
        {
            return await _db.PaymentMethods.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<PaymentMethod?> GetByCodeAsync(string code)
        {
            return await _db.PaymentMethods.FirstOrDefaultAsync(x => x.MethodCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code)
        {
            return await _db.PaymentMethods.AnyAsync(x => x.MethodCode == code);
        }

        public async Task<bool> CodeExistsAsync(string code, Guid excludeId)
        {
            return await _db.PaymentMethods.AnyAsync(x => x.MethodCode == code && x.Id != excludeId);
        }

        public async Task AddAsync(PaymentMethod method)
        {
            await _db.PaymentMethods.AddAsync(method);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentMethod method)
        {
            _db.PaymentMethods.Update(method);
            await _db.SaveChangesAsync();
        }
    }
}
