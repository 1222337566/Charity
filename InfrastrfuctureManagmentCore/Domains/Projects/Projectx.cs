using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastrfuctureManagmentCore.Domains.Projects
{
    public class Projectx
    {
        public Guid Id { get; set; }

        public string ProjectCode { get; set; } = string.Empty;
        public string NameAr { get; set; } = string.Empty;
        public string? NameEn { get; set; }

        public string? CustomerName { get; set; }
        public string? Location { get; set; }

        public DateTime? StartDateUtc { get; set; }
        public DateTime? EndDateUtc { get; set; }

        public decimal? EstimatedBudget { get; set; }

        public ProjectStatus Status { get; set; } = ProjectStatus.Planned;

        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
