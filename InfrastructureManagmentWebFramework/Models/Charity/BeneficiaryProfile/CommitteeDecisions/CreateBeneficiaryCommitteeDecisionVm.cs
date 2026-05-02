using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.CommitteeDecisions
{
    public class CreateBeneficiaryCommitteeDecisionVm
    {
        [Required]
        public Guid BeneficiaryId { get; set; }

        [Required(ErrorMessage = "تاريخ القرار مطلوب")]
        [Display(Name = "تاريخ القرار")]
        [DataType(DataType.Date)]
        public DateTime DecisionDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "نوع القرار مطلوب")]
        [Display(Name = "نوع القرار")]
        public string DecisionType { get; set; } = string.Empty;

        [Display(Name = "نوع المساعدة المعتمدة")]
        public Guid? ApprovedAidTypeId { get; set; }

        [Display(Name = "القيمة المعتمدة")]
        public decimal? ApprovedAmount { get; set; }

        [Display(Name = "المدة بالشهور")]
        public int? DurationInMonths { get; set; }

        [Display(Name = "ملاحظات اللجنة")]
        public string? CommitteeNotes { get; set; }

        [Display(Name = "حالة القرار")]
        public bool ApprovedStatus { get; set; } = true;

        public List<SelectListItem> AidTypes { get; set; } = new();
    }
}
