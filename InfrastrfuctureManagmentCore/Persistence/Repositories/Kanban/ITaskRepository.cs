using InfrastrfuctureManagmentCore.Domains.Kanaban;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban
{
    // Application/Abstractions/Kanban/ITaskRepository.cs
    public interface ITaskRepository
    {
        Task<List<TaskItem>> GetByBoardAsync(Guid boardId, CancellationToken ct);
        Task<TaskItem?> GetAsync(Guid id, CancellationToken ct);
        Task<TaskItem> CreateAsync(TaskItem task, CancellationToken ct);
        Task UpdateAsync(TaskItem task, CancellationToken ct);
        Task DeleteAsync(TaskItem task, CancellationToken ct);
    }
}
