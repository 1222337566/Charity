using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.OldRecords
{
    public class EditBeneficiaryOldRecordVm : CreateBeneficiaryOldRecordVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
