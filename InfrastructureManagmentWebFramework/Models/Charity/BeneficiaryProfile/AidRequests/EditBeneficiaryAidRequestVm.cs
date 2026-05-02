using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.AidRequests
{
    public class EditBeneficiaryAidRequestVm : CreateBeneficiaryAidRequestVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
