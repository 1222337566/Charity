using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries
{
    public class BeneficiaryListFilterVm
    {
        public string? Q { get; set; }
        public Guid? StatusId { get; set; }
        public bool? IsActive { get; set; }
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
