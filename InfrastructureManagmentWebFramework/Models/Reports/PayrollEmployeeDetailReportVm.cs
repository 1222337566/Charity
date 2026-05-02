using InfrastrfuctureManagmentCore.Queries.Reports;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Reports
{
    public class PayrollEmployeeDetailReportVm
    {
        public Guid? PayrollMonthId { get; set; }
        public List<SelectListItem> PayrollMonths { get; set; } = new();
        public List<PayrollEmployeeDetailReportRowDto> Rows { get; set; } = new();

        public decimal TotalNet => Rows.Sum(x => x.NetAmount);
        public decimal TotalBasic => Rows.Sum(x => x.BasicSalary);
        public decimal TotalAdditions => Rows.Sum(x => x.Additions);
        public decimal TotalDeductions => Rows.Sum(x => x.TotalDeductions);
    }
}
