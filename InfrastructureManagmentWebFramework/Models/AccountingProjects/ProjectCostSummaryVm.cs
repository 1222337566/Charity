using InfrastrfuctureManagmentCore.Domains.Accounting.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.AccountingProjects
{
    public class ProjectCostSummaryVm
    {
        public Guid? ProjectId { get; set; }
        public Guid? CostCenterId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
        public List<ProjectCostSummaryRowDto> Rows { get; set; } = new();
    }
}
