using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public interface IAdReader 
    {
        Task<IEnumerable<(string Id, string Name, string Email, string OU)>> FindUsersAsync(string q, int take, CancellationToken ct); 
    
    }
}
