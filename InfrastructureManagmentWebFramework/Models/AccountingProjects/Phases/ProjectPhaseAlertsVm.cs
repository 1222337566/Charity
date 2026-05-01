using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.AccountingProjects.Phases
{
    public class ProjectPhaseAlertsVm
    {
        public Guid? ProjectId { get; set; }
        public DateTime? AsOfDate { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<ProjectPhaseAlertRowDto> Rows { get; set; } = new();
    }
}
