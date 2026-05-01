using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public interface IDhcpReader
    {
        Task<IEnumerable<(string DeviceId, string IP, string Scope, string State)>> FindAsync(string q, int take, CancellationToken ct);
    }
}