using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Charity.Funders
{
    public class FunderListFilterVm
    {
        public string? Q { get; set; }
        public string? FunderType { get; set; }
        public bool? IsActive { get; set; }
        public List<SelectListItem> FunderTypes { get; set; } = new();
    }
}
