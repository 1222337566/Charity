using InfrastrfuctureManagmentCore.Domains.Progiling;
using InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public class AdDirectorySyncService : IAdDirectorySyncService
    {
        private readonly ILdapDirectoryService _ldap;
        private readonly IDirectoryUserRepository _repo;
        public AdDirectorySyncService(ILdapDirectoryService ldap, IDirectoryUserRepository repo)
        { _ldap = ldap; _repo = repo; }

        public async Task<int> RefreshAsync(string? query, string? ouDn, int take, CancellationToken ct)
        {
            var users = await _ldap.SearchUsersOnceAsync(query, ouDn, take, ct);
            var mapped = users.Select(u => new DirectoryUser
            {
                AdObjectId = u.AdObjectId,
                Upn = u.Upn,
                Email = u.Email,
                DisplayName = u.DisplayName,
                Department = u.Department,
                DistinguishedName = u.DistinguishedName,
                UserWorkstations = string.IsNullOrWhiteSpace(u.UserWorkstations)
                        ? null
                        : string.Join(",",
                            u.UserWorkstations.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                              .Select(s => s.Trim())
                                              .Distinct(StringComparer.OrdinalIgnoreCase)),
            });
            await _repo.UpsertRangeAsync(mapped, ct);
            return users.Count;
        }
    }
}
