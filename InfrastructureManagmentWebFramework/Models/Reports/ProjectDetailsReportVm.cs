using InfrastrfuctureManagmentCore.Queries.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class ProjectDetailsReportVm
    {
        public Guid? ProjectId { get; set; }
        public List<SelectListItem> Projects { get; set; } = new();
        public List<ProjectDetailsReportRowDto> Rows { get; set; } = new();

        public decimal TotalBudget => Rows.Sum(x => x.Budget);
        public decimal TotalActual => Rows.Sum(x => x.ActualBudgetLines);
        public decimal TotalAllocated => Rows.Sum(x => x.AllocatedGrants);
    }
}
