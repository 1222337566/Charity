using InfrastrfuctureManagmentCore.Domains.supplies;
using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class DirectoryDeviceRepository : IDirectoryDeviceRepository
    {
        private readonly AppDbContext _db;
        public DirectoryDeviceRepository(AppDbContext db) => _db = db;

        public async Task<List<DirectoryDevice>> GetByUserIdAsync(int userId, CancellationToken ct = default)
        {
            return await _db.DirectoryDevices
                .Where(d => d.DirectoryUserId == userId)
                .OrderByDescending(d => d.FetchedAtUtc)
                .ToListAsync(ct);
        }

        public async Task UpsertRangeAsync(IEnumerable<DirectoryDevice> devices, CancellationToken ct = default)
        {
            foreach (var d in devices)
            {
                var existing = await _db.DirectoryDevices
                    .FirstOrDefaultAsync(x => x.DirectoryUserId == d.DirectoryUserId && x.HostName == d.HostName, ct);

                if (existing != null)
                {
                    existing.IpAddress = d.IpAddress;
                    existing.MacAddress = d.MacAddress;
                    existing.Source = "DHCP";
                    existing.FetchedAtUtc = DateTime.UtcNow;
                }
                else
                {
                    d.Source = "DHCP";
                    d.FetchedAtUtc = DateTime.UtcNow;
                    _db.DirectoryDevices.Add(d);
                }
            }
            await _db.SaveChangesAsync(ct);
        }
    }
}
