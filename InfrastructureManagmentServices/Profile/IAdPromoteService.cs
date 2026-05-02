using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public interface IAdPromoteService
    {
        Task<int> PromoteAsync(IEnumerable<string> adObjectIds, CancellationToken ct);
    }
}
