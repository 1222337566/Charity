using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public class DeviceRepo : IDeviceRepo
    {
        private readonly AppDbContext _db;
        public DeviceRepo(AppDbContext db) => _db = db;

        public async Task<IEnumerable<(string Id, string Name, string IP, string MAC)>> FindAsync(string q, int take, CancellationToken ct)
        {
            q = q?.Trim() ?? "";
            var qr = _db.DirectoryDevices.AsNoTracking();

            // بحث بسيط بالاسم أو IP أو MAC
            if (!string.IsNullOrWhiteSpace(q))
            {
                qr = qr.Where(d =>
                    d.HostName.Contains(q) ||
                    d.IpAddress.Contains(q) ||
                    d.MacAddress.Contains(q));
            }

            var list = await qr.OrderByDescending(d => d.FetchedAtUtc)
                               .Take(take)
                               .Select(d => new { d.Id, d.HostName, d.IpAddress, d.MacAddress })
                               .ToListAsync(ct);

            return list.Select(d => (d.Id.ToString(), d.HostName, d.IpAddress, d.MacAddress));
        }
    }
}
