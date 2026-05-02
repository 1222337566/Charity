using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchListItemVm
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string? BeneficiaryCode { get; set; }
        public string ResearchNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string ApplicantName { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PriorityLevel { get; set; }
        public string? ReviewDecision { get; set; }
        public string? CommitteeDecision { get; set; }
    }

    public class CreateBeneficiaryHumanitarianResearchVm
    {
        [Required]
        [Display(Name = "المستفيد")]
        public Guid BeneficiaryId { get; set; }

        public List<SelectListItem> Beneficiaries { get; set; } = new();

        [Required]
        [Display(Name = "رقم البحث")]
        public string ResearchNumber { get; set; } = string.Empty;

        [Display(Name = "تاريخ الطلب")]
        public DateTime RequestDate { get; set; } = DateTime.Today;

        [Display(Name = "تاريخ البحث")]
        public DateTime? ResearchDate { get; set; }

        [Display(Name = "نوع المساعدة")]
        public string? AidTypeName { get; set; }

        [Required]
        [Display(Name = "اسم مقدم الطلب")]
        public string ApplicantName { get; set; } = string.Empty;

        [Display(Name = "جهة ورود الطلب")]
        public string? SourceOfRequest { get; set; }

        [Display(Name = "كود الباحث")]
        public string? ResearcherCode { get; set; }

        [Display(Name = "اسم الباحث")]
        public string? ResearcherName { get; set; }

        [Display(Name = "كود اللجنة")]
        public string? CommitteeCode { get; set; }

        [Display(Name = "درجة الأهمية")]
        public string? PriorityLevel { get; set; }

        [Required]
        [Display(Name = "اسم الحالة")]
        public string FullName { get; set; } = string.Empty;

        [Display(Name = "اسم الشهرة")]
        public string? NickName { get; set; }

        [Display(Name = "العمر")]
        public int? Age { get; set; }

        [Display(Name = "الحالة الاجتماعية")]
        public string? MaritalStatus { get; set; }

        [Display(Name = "الرقم القومي")]
        public string? NationalId { get; set; }

        [Display(Name = "العنوان بالتفصيل")]
        public string? AddressLine { get; set; }

        [Display(Name = "إجمالي أفراد الأسرة")]
        public int FamilyMembersCount { get; set; }

        [Display(Name = "إجمالي الدخل")]
        public decimal? TotalIncome { get; set; }

        [Display(Name = "إجمالي المصروفات")]
        public decimal? TotalExpenses { get; set; }

        [Display(Name = "متوسط الدخل")]
        public decimal? AverageIncome { get; set; }

        [Display(Name = "هل لدى الحالة مشروع قائم؟")]
        public bool HasExistingProject { get; set; }

        [Display(Name = "نوع المشروع القائم")]
        public string? ExistingProjectType { get; set; }

        [Display(Name = "حجم المشروع")]
        public string? ExistingProjectSize { get; set; }

        [Display(Name = "الاحتياجات المطلوبة")]
        public string? RequiredNeedsPrimary { get; set; }

        [Display(Name = "احتياجات أخرى")]
        public string? RequiredNeedsSecondary { get; set; }

        [Display(Name = "وصف السكن / المنزل")]
        public string? HousingDescription { get; set; }

        [Display(Name = "تقرير الباحث")]
        public string? ResearcherReport { get; set; }

        [Display(Name = "رأي مسؤول البحوث")]
        public string? ResearchManagerOpinion { get; set; }
    }

    public class ReviewBeneficiaryHumanitarianResearchVm
    {
        public Guid Id { get; set; }

        [Required]
        [Display(Name = "قرار المراجع")]
        public string Decision { get; set; } = string.Empty;

        [Required]
        [Display(Name = "الأسباب")]
        public string Reason { get; set; } = string.Empty;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Decisions { get; set; } = new();
    }

    public class CommitteeBeneficiaryHumanitarianResearchVm
    {
        public Guid Id { get; set; }

        [Display(Name = "تاريخ اجتماع اللجنة")]
        public DateTime CommitteeMeetingDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "قرار اللجنة")]
        public string Decision { get; set; } = string.Empty;

        [Display(Name = "نوع المساعدة المعتمدة")]
        public Guid? ApprovedAidTypeId { get; set; }

        // يحتفظ باسم النوع كنص للتوافق مع جدول تقييم اللجنة القديم
        // بينما يعتمد الربط التشغيلي على ApprovedAidTypeId.
        public string? ApprovedAidType { get; set; }

        [Display(Name = "القيمة المعتمدة")]
        public decimal? ApprovedAmount { get; set; }

        [Display(Name = "المدة بالشهور")]
        public int? DurationMonths { get; set; }

        [Display(Name = "ملاحظات اللجنة")]
        public string? Notes { get; set; }

        public List<SelectListItem> Decisions { get; set; } = new();
        public List<SelectListItem> AidTypes { get; set; } = new();
    }
}
