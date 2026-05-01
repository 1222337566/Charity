using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.CommitteeDecisions
{
    public class EditBeneficiaryCommitteeDecisionVm : CreateBeneficiaryCommitteeDecisionVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
