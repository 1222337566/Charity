using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Charity.Projects
{
    public class ProjectListFilterVm
    {
        public string? Q { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
