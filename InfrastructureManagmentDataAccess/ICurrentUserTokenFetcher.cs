using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess
{
    public interface ICurrentUserTokenFetcher
    {
        Task<string> GetTokenFromSelfAsync(CancellationToken ct = default);
    }
}
