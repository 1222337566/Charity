using InfrastrfuctureManagmentCore.Domains.Payments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Payments
{
    public interface IPaymentMethodRepository
    {
        Task<List<PaymentMethod>> GetAllAsync();
        Task<List<PaymentMethod>> GetActiveAsync();
        Task<PaymentMethod?> GetByIdAsync(Guid id);
        Task<PaymentMethod?> GetByCodeAsync(string code);

        Task<bool> CodeExistsAsync(string code);
        Task<bool> CodeExistsAsync(string code, Guid excludeId);

        Task AddAsync(PaymentMethod method);
        Task UpdateAsync(PaymentMethod method);
    }
}
