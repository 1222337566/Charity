using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class ProjectFinancialReportVm
    {
        public List<ProjectFinancialReportRowDto> Rows { get; set; } = new();
        public decimal TotalBudget => Rows.Sum(x => x.Budget);
        public decimal TotalAllocatedGrants => Rows.Sum(x => x.AllocatedGrants);
        public decimal TotalActual => Rows.Sum(x => x.ActualBudgetLines);
    }
}
