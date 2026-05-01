using InfrastrfuctureManagmentCore.Domains.Kanaban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Kanban
{
    // Application/Abstractions/Kanban/IKanbanService.cs
    public interface IKanbanService
    {
        Task<List<TaskItem>> GetBoardAsync(Guid boardId, CancellationToken ct);
        Task<TaskItem> CreateAsync(Guid boardId, string title, string? desc, string priority, string? assignedToUserId, DateTime? due, string currentUserId, CancellationToken ct);
        Task MoveAsync(Guid taskId, string newStatus, string currentUserId, CancellationToken ct);
        Task UpdateAsync(Guid taskId, string title, string? desc, string priority, string? assignedToUserId, DateTime? due, CancellationToken ct);
        Task DeleteAsync(Guid taskId, CancellationToken ct);
        Task<List<KanbanTask>> GetTasksAsync(Guid boardId, string userId);
        Task<KanbanTask> CreateTaskAsync(KanbanTask task, string userId);
        Task MoveTaskAsync(Guid taskId, string newStatus, int? orderIndex, string userId); // ← يدعم orderIndex
        Task UpdateTaskAsync(KanbanTask task, string userId);
        Task DeleteTaskAsync(Guid taskId, string userId);
    }
}
