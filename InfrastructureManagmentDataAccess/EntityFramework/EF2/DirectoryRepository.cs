using InfrastrfuctureManagmentCore.Domains.Progiling;
using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class DirectoryUserRepository : IDirectoryUserRepository
    {
        private readonly AppDbContext _db;
        public DirectoryUserRepository(AppDbContext db) => _db = db;

        public async Task UpsertRangeAsync(IEnumerable<DirectoryUser> users, CancellationToken ct)
        {
            // Upsert بسيط حسب AdObjectId
            var list = users.ToList();
            var ids = list.Select(x => x.AdObjectId).Distinct().ToList();
            var existing = await _db.DirectoryUsers
                .Where(x => ids.Contains(x.AdObjectId))
                .ToDictionaryAsync(x => x.AdObjectId, ct);

            foreach (var u in list)
            {
                if (existing.TryGetValue(u.AdObjectId, out var cur))
                {
                    cur.Upn = u.Upn;
                    cur.Email = u.Email;
                    cur.DisplayName = u.DisplayName;
                    cur.Department = u.Department;
                    cur.DistinguishedName = u.DistinguishedName;
                    cur.FetchedAtUtc = DateTime.UtcNow;
                    cur.UserWorkstations = u.UserWorkstations;
                }
                else
                {
                    u.FetchedAtUtc = DateTime.UtcNow;
                    _db.DirectoryUsers.Add(u);
                }
            }
            await _db.SaveChangesAsync(ct);
        }

        public Task<int> DeleteAllAsync(CancellationToken ct)
            => _db.Database.ExecuteSqlRawAsync("DELETE FROM DirectoryUsers", ct);

        public async Task<List<DirectoryUser>> SearchAsync(string? q, int skip, int take, CancellationToken ct)
        {
            var qry = _db.DirectoryUsers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(x =>
                    x.DisplayName!.Contains(q) || x.Email!.Contains(q) ||
                    x.Upn.Contains(q) || x.Department!.Contains(q));
            }
            return await qry
                .OrderBy(x => x.DisplayName)
                .Skip(skip).Take(take)
                .ToListAsync(ct);
        }

        public Task<int> CountAsync(string? q, CancellationToken ct)
        {
            var qry = _db.DirectoryUsers.AsQueryable();
            if (!string.IsNullOrWhiteSpace(q))
            {
                q = q.Trim();
                qry = qry.Where(x =>
                    x.DisplayName!.Contains(q) || x.Email!.Contains(q) ||
                    x.Upn.Contains(q) || x.Department!.Contains(q));
            }
            try
            {
                return qry.CountAsync(ct);
            }
            catch (Exception ex) {
                return qry.CountAsync(ct);
            }
        }

        public async Task<List<DirectoryUser>> GetByAdIdsAsync(IEnumerable<string> adIds, CancellationToken ct)
        {
            var ids = adIds.ToList();
            return await _db.DirectoryUsers.Where(x => ids.Contains(x.AdObjectId)).ToListAsync(ct);
        }
    }

}
