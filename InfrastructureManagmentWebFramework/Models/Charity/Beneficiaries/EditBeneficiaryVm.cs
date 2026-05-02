using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries
{
    public class EditBeneficiaryVm : CreateBeneficiaryVm
    {
        public Guid Id { get; set; }

        [Display(Name = "نشط")]
        public bool IsActive { get; set; } = true;
    }
}
