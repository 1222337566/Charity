using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.PosHolds;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.PosHolds
{
    public interface IPosHoldRepository
    {
        Task<List<PosHold>> GetHeldAsync();
        Task<PosHold?> GetByIdAsync(Guid id);
        Task<bool> HoldNumberExistsAsync(string holdNumber);
        Task AddAsync(PosHold hold);
        Task UpdateAsync(PosHold hold);
    }
}
