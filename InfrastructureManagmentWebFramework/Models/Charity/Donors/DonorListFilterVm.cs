using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Charity.Donors
{
    public class DonorListFilterVm
    {
        public string? Q { get; set; }
        public string? DonorType { get; set; }
        public bool? IsActive { get; set; }

        public List<SelectListItem> DonorTypes { get; set; } = new();
    }
}
