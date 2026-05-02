using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Kanaban
{
    // Domains/Kanban/KanbanTask.cs
   
        public class KanbanTask
        {
            public Guid Id { get; set; }
            public Guid BoardId { get; set; }
            public string Title { get; set; } = "";
            public string? Description { get; set; }

            // ToDo / InProgress / Done
            public string Status { get; set; } = "ToDo";

            public string Priority { get; set; } = "Medium";
            public string? AssignedToUserId { get; set; }
            public DateTime? DueDateUtc { get; set; }

            // NEW: ترتيب داخل العمود
            public int OrderIndex { get; set; } = 0;

            // (اختياري) تاريخ الإنشاء للمساعدة على الترتيب الاحتياطي
            public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        }    // ← ترتيب داخل العمود
    
}
