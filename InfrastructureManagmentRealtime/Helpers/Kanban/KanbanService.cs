using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Domains.Notify;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using InfrastructureManagmentCore.Services.Notifications;
using InfrastructureManagmentInfrastructure.Kanban;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentServices.Kanban
{
    public class KanbanService : IKanbanService
    {
        private readonly IKanbanRepository _repo;
          // اختياري لو مش عندك سيبه واحذف استعماله
           // اختياري للبث

        private readonly ITaskRepository _tasks;
        private readonly ITaskBoardRepository _boards;
        private readonly INotificationService _notify;               // عندك بالفعل
        private readonly IHubContext<KanbanHub> _hub;                 // SignalR
        public KanbanService(ITaskRepository tasks, ITaskBoardRepository boards, INotificationService notify, IHubContext<KanbanHub> hub, IKanbanRepository repo)
        {
            _tasks = tasks; _boards = boards; _notify = notify; _hub = hub;
            _repo = repo;
        }

        public Task<List<TaskItem>> GetBoardAsync(Guid boardId, CancellationToken ct)
            => _tasks.GetByBoardAsync(boardId, ct);

        public async Task<TaskItem> CreateAsync(Guid boardId, string title, string? desc, string priority, string? assignedToUserId, DateTime? due, string currentUserId, CancellationToken ct)
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
            task = await _tasks.CreateAsync(task, ct);

            // إشعار + بثّ
            if (!string.IsNullOrEmpty(assignedToUserId))
                await _notify.ToUserAsync(assignedToUserId, currentUserId, "New Task", title, $"/kanban?board={boardId}", "kanban-create", "info", ct);

            await _hub.Clients.Group(boardId.ToString()).SendAsync("TaskCreated", new
            {
                task.Id,
                task.Title,
                task.Description,
                task.Priority,
                task.Status,
                task.AssignedToUserId,
                task.DueDateUtc,
                task.BoardId
            }, ct);

            return task;
        }
        public Task<List<KanbanTask>> GetTasksAsync(Guid boardId, string userId)
           => _repo.GetTasksAsync(boardId, userId);

        public async Task<KanbanTask> CreateTaskAsync(KanbanTask task, string userId)
        {
            task.Status = NormalizeStatus(task.Status);
            var saved = await _repo.CreateTaskAsync(task, userId);

            // بث + إشعار (اختياري)
            await BroadcastAsync(saved.BoardId, "TaskCreated", new { taskId = saved.Id, status = saved.Status, orderIndex = saved.OrderIndex });
            if (!string.IsNullOrEmpty(saved.AssignedToUserId))
            {
                await SafeNotifyAsync(new NotificationMessage
                {
                    ToUserId = saved.AssignedToUserId!,
                    Title = "New task",
                    Message = $"'{saved.Title}' assigned to you",
                    Level = "info",
                    Url = $"/tasks/board?board={saved.BoardId}#task-{saved.Id}"
                });
            }

            return saved;
        }

        public async Task MoveTaskAsync(Guid taskId, string newStatus, int? orderIndex, string userId)
        {
            newStatus = NormalizeStatus(newStatus);

            await _repo.MoveTaskAsync(taskId, newStatus, orderIndex, userId);

            // نجيب المهمة بعد التحديث عشان نبث/نُشعِر بشكل صحيح
            var updated = await _repo.GetByIdAsync(taskId);
            if (updated is null) return;

            // بث (SignalR) للمجموعة الخاصة باللوح
            await BroadcastAsync(updated.BoardId, "TaskMoved", new
            {
                taskId = updated.Id,
                status = updated.Status,
                orderIndex = updated.OrderIndex
            });

            // إشعار للمكلّف (لو موجود)
            if (!string.IsNullOrEmpty(updated.AssignedToUserId))
            {
                await SafeNotifyAsync(new NotificationMessage
                {
                    ToUserId = updated.AssignedToUserId!,
                    Title = "Task moved",
                    Message = $"'{updated.Title}' → {updated.Status}",
                    Level = "info",
                    Url = $"/tasks/board?board={updated.BoardId}#task-{updated.Id}"
                });
            }
        }

        public async Task UpdateTaskAsync(KanbanTask dto, string userId)
        {
            if (!string.IsNullOrWhiteSpace(dto.Status))
                dto.Status = NormalizeStatus(dto.Status);

            await _repo.UpdateTaskAsync(dto, userId);

            var updated = await _repo.GetByIdAsync(dto.Id);
            if (updated is null) return;

            await BroadcastAsync(updated.BoardId, "TaskUpdated", new
            {
                taskId = updated.Id,
                status = updated.Status,
                orderIndex = updated.OrderIndex
            });

            // إشعار بسيط
            if (!string.IsNullOrEmpty(updated.AssignedToUserId))
            {
                await SafeNotifyAsync(new NotificationMessage
                {
                    ToUserId = updated.AssignedToUserId!,
                    Title = "Task updated",
                    Message = $"'{updated.Title}' updated",
                    Level = "info",
                    Url = $"/tasks/board?board={updated.BoardId}#task-{updated.Id}"
                });
            }
        }

        public async Task DeleteTaskAsync(Guid taskId, string userId)
        {
            // هات المهمة عشان نستعمل BoardId في البث بعد الحذف
            var task = await _repo.GetByIdAsync(taskId);
            if (task is null) return;

            await _repo.DeleteTaskAsync(taskId, userId);

            await BroadcastAsync(task.BoardId, "TaskDeleted", new { taskId });

            if (!string.IsNullOrEmpty(task.AssignedToUserId))
            {
                await SafeNotifyAsync(new NotificationMessage
                {
                    ToUserId = task.AssignedToUserId!,
                    Title = "Task deleted",
                    Message = $"'{task.Title}' was deleted",
                    Level = "warning"
                });
            }
        }

        // ==== Helpers ====

        private static string NormalizeStatus(string? s)
        {
            var k = (s ?? "").Trim().ToLowerInvariant();
            return k switch
            {
                "todo" or "upcoming" => "ToDo",
                "inprogress" or "in_progress" => "InProgress",
                "done" or "complete" or "completed" => "Done",
                _ => "ToDo"
            };
        }

        private async Task BroadcastAsync(Guid boardId, string eventName, object payload)
        {
            // لو مش موصل Hub في الـDI، احذف هذه الدالة واستدعاءاتها
            if (_hub is null) return;
            await _hub.Clients.Group($"board:{boardId}").SendAsync(eventName, payload);
        }

        private async Task SafeNotifyAsync(NotificationMessage msg)
        {
            // لو مش موصل INotificationService في الـDI، احذف الاستدعاءات لها
            if (_notify is null) return;
            try { await _notify.SendAsync(msg); }
            catch { /* لا تُسقط العملية الأساسية بسبب فشل إشعار */ }
        }

        public async Task MoveAsync(Guid taskId, string newStatus, string currentUserId, CancellationToken ct)
        {
            var t = await _tasks.GetAsync(taskId, ct) ?? throw new InvalidOperationException("Task not found");
            t.Status = newStatus; await _tasks.UpdateAsync(t, ct);

            await _hub.Clients.Group(t.BoardId.ToString()).SendAsync("TaskMoved", new { t.Id, t.BoardId, t.Status }, ct);
        }

        public async Task UpdateAsync(Guid taskId, string title, string? desc, string priority, string? assignedToUserId, DateTime? due, CancellationToken ct)
        {
            var t = await _tasks.GetAsync(taskId, ct) ?? throw new InvalidOperationException("Task not found");
            t.Title = title; t.Description = desc; t.Priority = priority; t.AssignedToUserId = assignedToUserId; t.DueDateUtc = due;
            await _tasks.UpdateAsync(t, ct);
            await _hub.Clients.Group(t.BoardId.ToString()).SendAsync("TaskUpdated", new
            {
                t.Id,
                t.Title,
                t.Description,
                t.Priority,
                t.AssignedToUserId,
                t.DueDateUtc
            }, ct);
        }

        public async Task DeleteAsync(Guid taskId, CancellationToken ct)
        {
            var t = await _tasks.GetAsync(taskId, ct) ?? throw new InvalidOperationException("Task not found");
            var boardId = t.BoardId;
            await _tasks.DeleteAsync(t, ct);
            await _hub.Clients.Group(boardId.ToString()).SendAsync("TaskDeleted", new { Id = taskId }, ct);
        }
    }
}
