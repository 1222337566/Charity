using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.FamilyMembers
{
    public class CreateBeneficiaryFamilyMemberVm
    {
        [Required]
        public Guid BeneficiaryId { get; set; }

        [Required(ErrorMessage = "الاسم مطلوب")]
        [Display(Name = "الاسم")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "صلة القرابة مطلوبة")]
        [Display(Name = "صلة القرابة")]
        public string Relationship { get; set; } = string.Empty;

        [Display(Name = "الرقم القومي")]
        public string? NationalId { get; set; }

        [Display(Name = "تاريخ الميلاد")]
        [DataType(DataType.Date)]
        public DateTime? BirthDate { get; set; }

        [Display(Name = "النوع")]
        public Guid? GenderId { get; set; }

        [Display(Name = "الحالة التعليمية")]
        public string? EducationStatus { get; set; }

        [Display(Name = "حالة العمل")]
        public string? WorkStatus { get; set; }

        [Display(Name = "الدخل الشهري")]
        public decimal? MonthlyIncome { get; set; }

        [Display(Name = "الحالة الصحية")]
        public string? HealthCondition { get; set; }

        [Display(Name = "معال")]
        public bool IsDependent { get; set; } = true;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> Genders { get; set; } = new();
    }
}
