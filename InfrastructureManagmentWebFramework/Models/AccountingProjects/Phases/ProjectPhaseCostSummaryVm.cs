using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases
{
    public class ProjectPhaseCostSummaryVm
    {
        public Guid? ProjectId { get; set; }
        public Guid? PhaseId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> Phases { get; set; } = new();
        public List<ProjectPhaseCostSummaryRowDto> Rows { get; set; } = new();
    }
}
