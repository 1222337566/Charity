using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Kanaban
{
    public class TaskBoard
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = default!;
        public string CreatedByUserId { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool? IsArchived { get; set; } = false;
        public ICollection<BoardUser> Members { get; set; } = new List<BoardUser>();
        public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
    }
}