using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class CreateAccountingIntegrationSourceDefinitionVm
    {
        [Required]
        [Display(Name = "كود المصدر")]
        [RegularExpression(@"^[A-Za-z0-9_.:-]+$", ErrorMessage = "الكود يسمح بالحروف الإنجليزية والأرقام والرموز . _ : - فقط")]
        public string SourceType { get; set; } = string.Empty;

        [Required]
        [Display(Name = "اسم المصدر")]
        public string NameAr { get; set; } = string.Empty;

        [Display(Name = "الاسم الإنجليزي")]
        public string? NameEn { get; set; }

        [Display(Name = "الموديول")]
        public string? ModuleCode { get; set; }

        [Display(Name = "الوصف")]
        public string? Description { get; set; }

        [Display(Name = "ترتيب العرض")]
        public int SortOrder { get; set; } = 100;

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "تفعيل التوليد الديناميكي للقيد")]
        public bool IsDynamicPostingEnabled { get; set; }

        [Display(Name = "اسم كلاس الحركة الكامل")]
        public string? EntityClrTypeName { get; set; }

        [Display(Name = "حقل المفتاح")]
        public string? IdPropertyName { get; set; } = "Id";

        [Display(Name = "حقل التاريخ")]
        public string? DatePropertyName { get; set; }

        [Display(Name = "حقل القيمة")]
        public string? AmountPropertyName { get; set; }

        [Display(Name = "حقل الرقم / المرجع")]
        public string? NumberPropertyName { get; set; }

        [Display(Name = "حقل البيان")]
        public string? TitlePropertyName { get; set; }

        [Display(Name = "قالب وصف القيد")]
        public string? DescriptionTemplate { get; set; }

        [Display(Name = "حقل الحساب المالي")]
        public string? FinancialAccountIdPropertyName { get; set; }

        [Display(Name = "حقل المشروع")]
        public string? ProjectIdPropertyName { get; set; }

        [Display(Name = "حقل مركز التكلفة")]
        public string? CostCenterIdPropertyName { get; set; }

        [Display(Name = "حقل كود الحدث")]
        public string? EventCodePropertyName { get; set; }

        [Display(Name = "حقل نوع التبرع")]
        public string? DonationTypePropertyName { get; set; }

        [Display(Name = "حقل نطاق التوجيه")]
        public string? TargetingScopeCodePropertyName { get; set; }

        [Display(Name = "حقل الغرض / الباب")]
        public string? PurposeNamePropertyName { get; set; }

        [Display(Name = "حقل نوع المساعدة")]
        public string? AidTypeIdPropertyName { get; set; }

        [Display(Name = "حقل نوع الحركة المخزنية")]
        public string? StoreMovementTypePropertyName { get; set; }
    }
}
