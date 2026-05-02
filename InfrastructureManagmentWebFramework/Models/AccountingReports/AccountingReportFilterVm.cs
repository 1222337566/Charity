using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.AccountingReports
{
    public class AccountingReportFilterVm
    {
        [Display(Name = "الفترة المالية")]
        public Guid? FiscalPeriodId { get; set; }

        [Display(Name = "الحساب")]
        public Guid? AccountId { get; set; }

        [Display(Name = "مركز التكلفة")]
        public Guid? CostCenterId { get; set; }

        [Display(Name = "من تاريخ")]
        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [Display(Name = "إلى تاريخ")]
        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        public List<SelectListItem> FiscalPeriods { get; set; } = new();
        public List<SelectListItem> Accounts { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
    }
}
