using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles
{
    public class AidCycleBatchDisbursementVm
    {
        public Guid AidCycleId { get; set; }
        public string CycleDisplay { get; set; } = string.Empty;

        [Display(Name = "تاريخ الصرف")]
        [DataType(DataType.Date)]
        public DateTime DisbursementDate { get; set; } = DateTime.Today;

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Display(Name = "الحساب المالي")]
        public Guid? FinancialAccountId { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
        public List<AidCycleBeneficiaryListItemVm> EligibleLines { get; set; } = new();
        public List<Guid> SelectedLineIds { get; set; } = new();
    }
}
