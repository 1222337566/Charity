using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Assessments
{
    public class CreateBeneficiaryAssessmentVm
    {
        [Required]
        public Guid BeneficiaryId { get; set; }

        [Required(ErrorMessage = "تاريخ الزيارة مطلوب")]
        [Display(Name = "تاريخ الزيارة")]
        [DataType(DataType.Date)]
        public DateTime VisitDate { get; set; } = DateTime.Today;

        [Display(Name = "وصف حالة السكن")]
        public string? HousingCondition { get; set; }

        [Display(Name = "الوضع الاقتصادي")]
        public string? EconomicCondition { get; set; }

        [Display(Name = "الوضع الصحي")]
        public string? HealthCondition { get; set; }

        [Display(Name = "الوضع الاجتماعي")]
        public string? SocialCondition { get; set; }

        [Display(Name = "نوع المساعدة المقترح")]
        public Guid? RecommendedAidTypeId { get; set; }

        [Display(Name = "القيمة المقترحة")]
        public decimal? RecommendationAmount { get; set; }

        [Display(Name = "درجة التقييم")]
        public decimal? AssessmentScore { get; set; }

        [Display(Name = "التوصية")]
        public string? RecommendationText { get; set; }

        [Display(Name = "القرار المقترح")]
        public string? DecisionSuggested { get; set; }

        public List<SelectListItem> AidTypes { get; set; } = new();
    }
}
