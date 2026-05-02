using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.FunderProfile.GrantInstallments
{
    public class CreateGrantInstallmentVm
    {
        public Guid GrantAgreementId { get; set; }

        [Display(Name = "رقم الدفعة")]
        public int InstallmentNumber { get; set; } = 1;

        [Display(Name = "تاريخ الاستحقاق")]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; } = DateTime.Today;

        [Display(Name = "قيمة الدفعة")]
        public decimal Amount { get; set; }

        [Display(Name = "المبلغ المستلم")]
        public decimal? ReceivedAmount { get; set; }

        [Display(Name = "تاريخ الاستلام")]
        [DataType(DataType.Date)]
        public DateTime? ReceivedDate { get; set; }

        [Display(Name = "طريقة الدفع")]
        public Guid? PaymentMethodId { get; set; }

        [Display(Name = "الحساب المالي")]
        public Guid? FinancialAccountId { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Planned";

        [Display(Name = "رقم مرجعي")]
        public string? ReferenceNumber { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Statuses { get; set; } = new();
        public List<SelectListItem> PaymentMethods { get; set; } = new();
        public List<SelectListItem> FinancialAccounts { get; set; } = new();
    }
}
