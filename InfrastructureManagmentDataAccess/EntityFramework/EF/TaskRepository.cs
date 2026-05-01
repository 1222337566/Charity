using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    // Infrastructure/Repos/Kanban/TaskRepository.cs
    public class TaskRepository : ITaskRepository
    {
        private readonly AppDbContext _db;
        public TaskRepository(AppDbContext db) => _db = db;

        public Task<List<TaskItem>> GetByBoardAsync(Guid boardId, CancellationToken ct)
            => _db.TaskItems.Where(t => t.BoardId == boardId)
                            .OrderBy(t => t.CreatedAtUtc).ToListAsync(ct);

        public Task<TaskItem?> GetAsync(Guid id, CancellationToken ct)
            => _db.TaskItems.FirstOrDefaultAsync(t => t.Id == id, ct);

        public async Task<TaskItem> CreateAsync(TaskItem task, CancellationToken ct)
        {
            _db.TaskItems.Add(task);
            await _db.SaveChangesAsync(ct);
            return task;
        }

        public async Task UpdateAsync(TaskItem task, CancellationToken ct)
        {
            task.UpdatedAtUtc = DateTime.UtcNow;
            _db.TaskItems.Update(task);
            await _db.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(TaskItem task, CancellationToken ct)
        {
            _db.TaskItems.Remove(task);
            await _db.SaveChangesAsync(ct);
        }
    }
}
