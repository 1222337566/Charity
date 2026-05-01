using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Progiling;
namespace InfrastrfuctureManagmentCore.Persistence.Repositories.DirectoryUser
{
    public interface IDirectoryUserRepository
    {
        Task UpsertRangeAsync(IEnumerable<InfrastrfuctureManagmentCore.Domains.Progiling.DirectoryUser> users, CancellationToken ct);
        Task<List<InfrastrfuctureManagmentCore.Domains.Progiling.DirectoryUser>> SearchAsync(string? q, int skip, int take, CancellationToken ct);
        Task<int> CountAsync(string? q, CancellationToken ct);
        Task<List<InfrastrfuctureManagmentCore.Domains.Progiling.DirectoryUser>> GetByAdIdsAsync(IEnumerable<string> adIds, CancellationToken ct);
        Task<int> DeleteAllAsync(CancellationToken ct); // اختياري، لتنظيف كامل
    }
}
