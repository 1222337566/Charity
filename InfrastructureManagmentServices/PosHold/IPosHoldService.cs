using InfrastructureManagmentWebFramework.Models.POS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.PosHolds
{
    public interface IPosHoldService
    {
        Task HoldAsync(PosSaleVm vm);
        Task<PosSaleVm?> ResumeAsync(Guid holdId);
        Task MarkCompletedAsync(Guid holdId);
    }
}
