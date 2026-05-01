using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.Documents
{
    public class EditBeneficiaryDocumentVm : CreateBeneficiaryDocumentVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
