using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.BeneficiaryProfile.FamilyMembers
{
    public class EditBeneficiaryFamilyMemberVm : CreateBeneficiaryFamilyMemberVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
