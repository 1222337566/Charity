using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Beneficiaries
{
    public class CreateProjectBeneficiaryVm
    {
        public Guid ProjectId { get; set; }

        [Required][Display(Name = "المستفيد")]
        public Guid BeneficiaryId { get; set; }

        [Display(Name = "الفئة المستهدفة")]
        public string? TargetGroupName { get; set; }

        [Display(Name = "المرحلة")]
        public Guid? PhaseId { get; set; }

        [Display(Name = "النشاط المرتبط")]
        public Guid? ActivityId { get; set; }

        [Display(Name = "نوع المشاركة")]
        public string ParticipationType { get; set; } = "Beneficiary";

        [Display(Name = "تاريخ الإلحاق")]
        [DataType(DataType.Date)]
        public DateTime EnrollmentDate { get; set; } = DateTime.Today;

        [Display(Name = "تاريخ الخروج")]
        [DataType(DataType.Date)]
        public DateTime? ExitDate { get; set; }

        [Display(Name = "نوع الاستفادة")]
        public string? BenefitType { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        // ── Lookups ──
        public List<SelectListItem> Beneficiaries   { get; set; } = new();
        public List<SelectListItem> TargetGroups    { get; set; } = new();
        public List<SelectListItem> Phases          { get; set; } = new();
        public List<SelectListItem> Activities      { get; set; } = new();
        public List<SelectListItem> ParticipationTypes { get; set; } = new()
        {
            new("مشارك",  "Participant"),
            new("منتفع",  "Beneficiary")
        };
    }
}
