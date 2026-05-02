using InfrastructureManagmentWebFramework.DTOs.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Profile
{
    public interface ILdapDirectoryService
    {
        Task<bool> PingAsync(CancellationToken ct);
        public Task<(IReadOnlyList<AdUserDto> users, string? nextCookie)> SearchUsersAsync(string? query, string? ouDn, string? pageCookie, int pageSize, CancellationToken ct);
        Task<IReadOnlyList<AdUserDto>> SearchUsersOnceAsync(string? query, string? ouDn, int take, CancellationToken ct); // للمعاينة السريعة
    }
}
