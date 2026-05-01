using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.ProjectProfile.Grants
{
    public class CreateProjectGrantVm
    {
        public Guid ProjectId { get; set; }

        [Required]
        [Display(Name = "اتفاقية التمويل")]
        public Guid GrantAgreementId { get; set; }

        [Display(Name = "المبلغ المخصص")]
        public decimal AllocatedAmount { get; set; }

        [Display(Name = "تاريخ التخصيص")]
        [DataType(DataType.Date)]
        public DateTime AllocatedDate { get; set; } = DateTime.Today;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }

        public List<SelectListItem> GrantAgreements { get; set; } = new();
    }
}
