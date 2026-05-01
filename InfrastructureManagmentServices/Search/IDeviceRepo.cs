using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public interface IDeviceRepo
    { 
        Task<IEnumerable<(string Id, string Name, string IP, string MAC)>> FindAsync(string q, int take, CancellationToken ct);
    }

}
