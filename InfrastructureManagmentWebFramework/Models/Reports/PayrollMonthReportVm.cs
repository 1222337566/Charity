using InfrastrfuctureManagmentCore.Queries.Reports;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class PayrollMonthReportVm
    {
        public List<PayrollMonthReportRowDto> Rows { get; set; } = new();
        public decimal TotalNet => Rows.Sum(x => x.TotalNet);
    }
}
