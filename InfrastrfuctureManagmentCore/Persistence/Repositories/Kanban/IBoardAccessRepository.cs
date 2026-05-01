using System;
using System.Threading.Tasks;

namespace InfrastructureManagmentCore.Kanban
{
    public interface IBoardAccessRepository
    {
        /// <summary>هل المستخدم عضو في هذا اللوح؟</summary>
        Task<bool> IsMemberAsync(Guid boardId, string userId);

        /// <summary>ارمِ UnauthorizedAccessException إن لم يكن عضوًا.</summary>
        Task EnsureMemberAsync(Guid boardId, string userId);
    }
}