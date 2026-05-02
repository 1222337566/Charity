using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Kanaban
{
    // Domains/Kanban/TaskItem.cs
    public class TaskItem
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid BoardId { get; set; }
        public TaskBoard Board { get; set; } = default!;

        public string Title { get; set; } = default!;
        public string? Description { get; set; }

        // ToDo | InProgress | Done | Blocked
        public string Status { get; set; } = "ToDo";

        // Low | Medium | High | Critical
        public string Priority { get; set; } = "Medium";

        public string? AssignedToUserId { get; set; }
        public DateTime? DueDateUtc { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
