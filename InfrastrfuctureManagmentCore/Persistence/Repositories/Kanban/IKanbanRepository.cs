using InfrastrfuctureManagmentCore.Domains.Kanaban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban
{
    public interface IKanbanRepository
    {
        Task<List<KanbanTask>> GetTasksAsync(Guid boardId, string userId);
        Task<KanbanTask> CreateTaskAsync(KanbanTask task, string userId);
        Task MoveTaskAsync(Guid taskId, string newStatus, int? orderIndex, string userId);
        Task UpdateTaskAsync(KanbanTask task, string userId);
        Task DeleteTaskAsync(Guid taskId, string userId);
        Task<KanbanTask?> GetByIdAsync(Guid taskId);
    }
}
