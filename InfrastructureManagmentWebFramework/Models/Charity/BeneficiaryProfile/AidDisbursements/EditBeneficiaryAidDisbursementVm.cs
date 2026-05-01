using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidDisbursements
{
    public class EditBeneficiaryAidDisbursementVm : CreateBeneficiaryAidDisbursementVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
