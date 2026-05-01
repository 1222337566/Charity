using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles
{
    public class EditAidCycleBeneficiaryVm
    {
        public Guid Id { get; set; }
        public Guid AidCycleId { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string BeneficiaryDisplay { get; set; } = string.Empty;

        [Display(Name = "القيمة المجدولة")]
        public decimal? ScheduledAmount { get; set; }

        [Display(Name = "القيمة المعتمدة")]
        public decimal? ApprovedAmount { get; set; }

        [Display(Name = "الحالة")]
        public string Status { get; set; } = "Eligible";

        [Display(Name = "آخر صرف")]
        [DataType(DataType.Date)]
        public DateTime? LastDisbursementDate { get; set; }

        [Display(Name = "الاستحقاق القادم")]
        [DataType(DataType.Date)]
        public DateTime? NextDueDate { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        [Display(Name = "سبب الإيقاف/التأجيل")]
        public string? StopReason { get; set; }
    }
}
