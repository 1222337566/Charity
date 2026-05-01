using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;

namespace InfrastructureManagmentServices.Kanban
{
    /// <summary>
    /// تنفيذ مبسط — يعتمد فقط على ITaskRepository و ITaskBoardRepository
    /// بدون SignalR أو IKanbanRepository
    /// </summary>
    public class SimpleKanbanService : IKanbanService
    {
        private readonly ITaskRepository _tasks;
        private readonly ITaskBoardRepository _boards;

        public SimpleKanbanService(
            ITaskRepository tasks,
            ITaskBoardRepository boards)
        {
            _tasks = tasks;
            _boards = boards;
        }

        // ── TaskItem methods (used by MyBoardController) ──

        public Task<List<TaskItem>> GetBoardAsync(Guid boardId, CancellationToken ct)
            => _tasks.GetByBoardAsync(boardId, ct);

        public async Task<TaskItem> CreateAsync(
            Guid boardId, string title, string? desc,
            string priority, string? assignedToUserId,
            DateTime? due, string currentUserId, CancellationToken ct)
        {
            var task = new TaskItem
            {
                BoardId = boardId,
                Title = title,
                Description = desc,
                Priority = priority,
                AssignedToUserId = assignedToUserId,
                DueDateUtc = due,
                Status = "ToDo"
            };
            return await _tasks.CreateAsync(task, ct);
        }

        public async Task MoveAsync(Guid taskId, string newStatus,
            string currentUserId, CancellationToken ct)
        {
            var t = await _tasks.GetAsync(taskId, ct);
            if (t == null) return;
            t.Status = Normalize(newStatus);
            t.UpdatedAtUtc = DateTime.UtcNow;
            await _tasks.UpdateAsync(t, ct);
        }

        public async Task UpdateAsync(Guid taskId, string title, string? desc,
            string priority, string? assignedToUserId,
            DateTime? due, CancellationToken ct)
        {
            var t = await _tasks.GetAsync(taskId, ct);
            if (t == null) return;
            t.Title = title; t.Description = desc; t.Priority = priority;
            t.AssignedToUserId = assignedToUserId; t.DueDateUtc = due;
            t.UpdatedAtUtc = DateTime.UtcNow;
            await _tasks.UpdateAsync(t, ct);
        }

        public async Task DeleteAsync(Guid taskId, CancellationToken ct)
        {
            var t = await _tasks.GetAsync(taskId, ct);
            if (t != null) await _tasks.DeleteAsync(t, ct);
        }

        // ── KanbanTask methods — stub (not used by MyBoard) ──

        public Task<List<KanbanTask>> GetTasksAsync(Guid boardId, string userId)
            => Task.FromResult(new List<KanbanTask>());

        public Task<KanbanTask> CreateTaskAsync(KanbanTask task, string userId)
            => Task.FromResult(task);

        public Task MoveTaskAsync(Guid taskId, string newStatus, int? orderIndex, string userId)
            => Task.CompletedTask;

        public Task UpdateTaskAsync(KanbanTask task, string userId)
            => Task.CompletedTask;

        public Task DeleteTaskAsync(Guid taskId, string userId)
            => Task.CompletedTask;

        // ── helper ──
        private static string Normalize(string? s)
            => (s ?? "").Trim().ToLowerInvariant() switch
            {
                "todo" or "upcoming" => "ToDo",
                "inprogress" or "in_progress" => "InProgress",
                "blocked" => "Blocked",
                "done" or "complete" => "Done",
                _ => s ?? "ToDo"
            };
    }
}
