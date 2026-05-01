using InfrastrfuctureManagmentCore.Domains.supplies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser
{
    public interface IDirectoryDeviceRepository
    {
        Task UpsertRangeAsync(IEnumerable<DirectoryDevice> devices, CancellationToken ct = default);
        Task<List<DirectoryDevice>> GetByUserIdAsync(int userId, CancellationToken ct = default);
    }
}
