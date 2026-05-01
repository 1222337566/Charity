using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Kanaban
{
    public class TaskAudit
    {
        public long Id { get; set; }
        public Guid TaskId { get; set; }
        public string Action { get; set; } = ""; // Created/Updated/Moved/Deleted
        public string? FromStatus { get; set; }
        public string? ToStatus { get; set; }
        public string ByUserId { get; set; } = "";
        public DateTime AtUtc { get; set; } = DateTime.UtcNow;
    }
}
