using InfrastrfuctureManagmentCore.Domains.Kanaban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban
{
    // Application/Abstractions/Kanban/ITaskBoardRepository.cs
    public interface ITaskBoardRepository
    {
        Task<TaskBoard?> GetAsync(Guid id, CancellationToken ct);
        Task<List<TaskBoard>> ListMineAsync(string userId, CancellationToken ct);
        Task<TaskBoard> CreateAsync(TaskBoard board, CancellationToken ct);
        Task<bool> IsOwnerAsync(Guid boardId, string userId, CancellationToken ct);
        Task<bool> IsMemberAsync(Guid boardId, string userId, CancellationToken ct);
        Task AddMemberAsync(Guid boardId, string targetUserId, string role, string byUserId, CancellationToken ct);
        Task RemoveMemberAsync(Guid boardId, string targetUserId, string byUserId, CancellationToken ct);
        Task<List<InfrastrfuctureManagmentCore.Domains.Kanaban.BoardUser>> GetMembersAsync(Guid boardId, string byUserId, CancellationToken ct);
    }
}
