using InfrastructureManagmentWebFramework.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public interface IAdImportService
    {
        Task<int> ImportUsersAsync(IEnumerable<AdUserDto> users, CancellationToken ct); // Upsert إلى ApplicationUser
    }
}
