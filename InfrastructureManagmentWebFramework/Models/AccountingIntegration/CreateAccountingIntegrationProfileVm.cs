using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class CreateAccountingIntegrationProfileVm
    {
        [Required]
        [Display(Name = "نوع المصدر")]
        public string SourceType { get; set; } = string.Empty;

        [Display(Name = "كود الحدث")]
        public string? EventCode { get; set; }

        [Display(Name = "اسم الحدث")]
        public string? EventNameAr { get; set; }

        [Required]
        [Display(Name = "اسم الربط")]
        public string ProfileNameAr { get; set; } = string.Empty;

        [Display(Name = "نوع التبرع")]
        public string? MatchDonationType { get; set; }

        [Display(Name = "نطاق التوجيه")]
        public string? MatchTargetingScopeCode { get; set; }

        [Display(Name = "الغرض / الباب")]
        public string? MatchPurposeName { get; set; }

        [Display(Name = "نوع المساعدة")]
        public Guid? MatchAidTypeId { get; set; }

        [Display(Name = "نوع الحركة المخزنية")]
        public string? MatchStoreMovementType { get; set; }

        [Display(Name = "الأولوية")]
        public int Priority { get; set; } = 100;

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "حساب المدين")]
        public Guid? DebitAccountId { get; set; }

        [Display(Name = "حساب الدائن")]
        public Guid? CreditAccountId { get; set; }

        [Display(Name = "استخدم الحساب المالي من الحركة كمدين")]
        public bool UseSourceFinancialAccountAsDebit { get; set; }

        [Display(Name = "استخدم الحساب المالي من الحركة كدائن")]
        public bool UseSourceFinancialAccountAsCredit { get; set; }

        [Display(Name = "مركز التكلفة الافتراضي")]
        public Guid? DefaultCostCenterId { get; set; }

        [Display(Name = "ترحيل تلقائي")]
        public bool AutoPost { get; set; } = true;

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        public List<SelectListItem> SourceTypes { get; set; } = new();
        public List<SelectListItem> Accounts { get; set; } = new();
        public List<SelectListItem> CostCenters { get; set; } = new();
        public List<SelectListItem> AidTypes { get; set; } = new();
        public List<SelectListItem> DonationTypes { get; set; } = new();
        public List<SelectListItem> TargetingScopes { get; set; } = new();
        public List<SelectListItem> StoreMovementTypes { get; set; } = new();
    }
}
