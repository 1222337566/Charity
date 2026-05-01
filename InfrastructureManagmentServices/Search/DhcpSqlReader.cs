using InfrastructureManagmentDataAccess.EntityFramework;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Search
{
    public class DhcpSqlReader : IDhcpReader
    {
        private readonly AppDbContext _db;
        public DhcpSqlReader(AppDbContext db) => _db = db;

        public async Task<IEnumerable<(string DeviceId, string IP, string Scope, string State)>> FindAsync(string q, int take, CancellationToken ct)
        {
            q = q?.Trim() ?? "";
            var qr = _db.DhcpRecords.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q))
            {
                qr = qr.Where(x => x.IP.Contains(q) || x.Scope.Contains(q) || x.MAC.Contains(q));
            }

            var list = await qr.OrderByDescending(x => x.UpdatedAt)
                               .Take(take)
                               .Select(x => new { x.DeviceId, x.IP, x.Scope, x.State })
                               .ToListAsync(ct);

            return list.Select(x => (x.DeviceId ?? "", x.IP, x.Scope, x.State));
        }
    }
}
