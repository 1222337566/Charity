using InfrastrfuctureManagmentCore.Domains.Kanaban;
using InfrastrfuctureManagmentCore.Domains.Notify;
using InfrastrfuctureManagmentCore.Persistence.Repositories.Kanban;
using InfrastructureManagmentCore.Services.Notifications;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class KanbanRepository : IKanbanRepository
    {
        private readonly AppDbContext _db;
        private readonly INotificationService _notifications; // للبند 15

        public KanbanRepository(AppDbContext db, INotificationService notifications)
        {
            _db = db; _notifications = notifications;
        }

        private async Task EnsureBoardAccess(Guid boardId, string userId)
        {
            var ok = await _db.boardUsers.AnyAsync(x => x.BoardId == boardId && x.UserId == userId);
            if (!ok) throw new UnauthorizedAccessException("You are not a member of this board.");
        }

        private async Task AddAuditAsync(Guid taskId, string action, string byUserId, string? from = null, string? to = null)
        {
            _db.TaskAudits.Add(new TaskAudit
            {
                TaskId = taskId,
                Action = action,
                ByUserId = byUserId,
                FromStatus = from,
                ToStatus = to,
                AtUtc = DateTime.UtcNow
            });
            await _db.SaveChangesAsync();
        }

        public async Task<List<KanbanTask>> GetTasksAsync(Guid boardId, string userId)
        {
            await EnsureBoardAccess(boardId, userId);
            return await _db.kanbanTasks
                .Where(t => t.BoardId == boardId)
                .OrderBy(t => t.Status)
                .ThenBy(t => t.OrderIndex)
                .ThenBy(t => t.CreatedAtUtc)
                .ToListAsync();
        }
        // Infrastructure/Kanban/KanbanRepository.cs (تنفيذ)
        public async Task<KanbanTask?> GetByIdAsync(Guid taskId)
            => await _db.kanbanTasks.FirstOrDefaultAsync(x => x.Id == taskId);
        public async Task<KanbanTask> CreateTaskAsync(KanbanTask task, string userId)
        {
            await EnsureBoardAccess(task.BoardId, userId);

            // اجلب آخر Index في العمود نفسه
            task.OrderIndex = await _db.kanbanTasks
                .Where(x => x.BoardId == task.BoardId && x.Status == task.Status)
                .Select(x => (int?)x.OrderIndex).MaxAsync() ?? 0;
            task.OrderIndex++;

            _db.kanbanTasks.Add(task);
            await _db.SaveChangesAsync();

            await AddAuditAsync(task.Id, "Created", userId);

            // إشعار (اختياري): لو فيه AssignedToUserId مختلف عن المنشئ
            if (!string.IsNullOrEmpty(task.AssignedToUserId) && task.AssignedToUserId != userId)
            {
                await _notifications.SendAsync(new NotificationMessage
                {
                    ToUserId = task.AssignedToUserId,
                    Title = "New Task",
                    Message = $"Task '{task.Title}' assigned to you.",
                    Level = "info",
                    Url = $"/tasks/board?board={task.BoardId}#task-{task.Id}"
                });
            }

            return task;
        }

        public async Task MoveTaskAsync(Guid taskId, string newStatus, int? orderIndex, string userId)
        {
            var t = await _db.kanbanTasks.FindAsync(taskId)
                ?? throw new InvalidOperationException("Task not found");

            await EnsureBoardAccess(t.BoardId, userId);
            var oldStatus = t.Status;

            if (!string.Equals(oldStatus, newStatus, StringComparison.OrdinalIgnoreCase))
                t.Status = newStatus;

            // عمود الهدف بدون التسك الحالي
            var col = await _db.kanbanTasks
                .Where(x => x.BoardId == t.BoardId && x.Status == t.Status && x.Id != t.Id)
                .OrderBy(x => x.OrderIndex)
                .ToListAsync();

            var target = new List<KanbanTask>(col);
            var idx = Math.Clamp(orderIndex ?? col.Count, 0, col.Count);
            target.Insert(idx, t);

            for (int i = 0; i < target.Count; i++)
                target[i].OrderIndex = i;

            await _db.SaveChangesAsync();

            await AddAuditAsync(t.Id, "Moved", userId, oldStatus, t.Status);

            // إشعار للمكلّف
            if (!string.IsNullOrEmpty(t.AssignedToUserId))
            {
                await _notifications.SendAsync(new NotificationMessage
                {
                    ToUserId = t.AssignedToUserId,
                    Title = "Task moved",
                    Message = $"'{t.Title}' → {t.Status}",
                    Level = "info",
                    Url = $"/tasks/board?board={t.BoardId}#task-{t.Id}"
                });
            }
        }

        public async Task UpdateTaskAsync(KanbanTask dto, string userId)
        {
            var t = await _db.kanbanTasks.FindAsync(dto.Id)
                ?? throw new InvalidOperationException("Task not found");

            await EnsureBoardAccess(t.BoardId, userId);

            var oldStatus = t.Status;
            var oldAssignee = t.AssignedToUserId;

            t.Title = dto.Title ?? t.Title;
            t.Description = dto.Description ?? t.Description;
            t.Priority = dto.Priority ?? t.Priority;
            t.DueDateUtc = dto.DueDateUtc ?? t.DueDateUtc;

            if (!string.IsNullOrWhiteSpace(dto.Status) && dto.Status != t.Status)
                t.Status = dto.Status;

            if (dto.AssignedToUserId != null && dto.AssignedToUserId != t.AssignedToUserId)
                t.AssignedToUserId = dto.AssignedToUserId;

            await _db.SaveChangesAsync();
            await AddAuditAsync(t.Id, "Updated", userId,
                oldStatus != t.Status ? oldStatus : null,
                oldStatus != t.Status ? t.Status : null);

            // إشعار لو تغيّر المكلّف أو الحالة
            if (t.AssignedToUserId != oldAssignee || oldStatus != t.Status)
            {
                var toUser = t.AssignedToUserId ?? oldAssignee;
                if (!string.IsNullOrEmpty(toUser))
                {
                    await _notifications.SendAsync(new NotificationMessage
                    {
                        ToUserId = toUser,
                        Title = "Task updated",
                        Message = $"'{t.Title}' updated",
                        Level = "info",
                        Url = $"/tasks/board?board={t.BoardId}#task-{t.Id}"
                    });
                }
            }
        }

        public async Task DeleteTaskAsync(Guid taskId, string userId)
        {
            var t = await _db.kanbanTasks.FindAsync(taskId)
                ?? throw new InvalidOperationException("Task not found");

            await EnsureBoardAccess(t.BoardId, userId);

            _db.kanbanTasks.Remove(t);
            await _db.SaveChangesAsync();
            await AddAuditAsync(taskId, "Deleted", userId);

            // إشعار اختياري للمكلّف الحالي
            if (!string.IsNullOrEmpty(t.AssignedToUserId))
            {
                await _notifications.SendAsync(new NotificationMessage
                {
                    ToUserId = t.AssignedToUserId,
                    Title = "Task deleted",
                    Message = $"'{t.Title}' was deleted",
                    Level = "warning"
                });
            }
        }
    }
}
