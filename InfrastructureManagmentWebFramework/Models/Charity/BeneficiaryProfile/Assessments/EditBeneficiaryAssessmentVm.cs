using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Assessments
{
    public class EditBeneficiaryAssessmentVm : CreateBeneficiaryAssessmentVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
