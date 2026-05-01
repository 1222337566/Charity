using InfrastrfuctureManagmentCore.Domains.supplies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public interface INetworkDeviceService
    {
        Task<List<DirectoryDevice>> DiscoverDevicesFromDhcpAsync(int userId, string userWorkstations, CancellationToken ct = default);
    }
}
