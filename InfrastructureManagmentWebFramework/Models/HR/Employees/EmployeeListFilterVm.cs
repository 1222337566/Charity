using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.HR.Employees
{
    public class EmployeeListFilterVm
    {
        public string? Q { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public List<SelectListItem> Statuses { get; set; } = new();
    }
}
