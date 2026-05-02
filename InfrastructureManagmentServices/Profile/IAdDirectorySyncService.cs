using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public interface IAdDirectorySyncService
    {
        Task<int> RefreshAsync(string? query, string? ouDn, int take, CancellationToken ct);
    }
}
