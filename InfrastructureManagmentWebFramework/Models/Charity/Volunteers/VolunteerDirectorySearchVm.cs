using Microsoft.AspNetCore.Mvc.Rendering;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerDirectorySearchVm
    {
        public string? Q { get; set; }
        public Guid? SkillDefinitionId { get; set; }
        public string? Area { get; set; }
        public bool ActiveOnly { get; set; } = true;
        public List<SelectListItem> SkillDefinitions { get; set; } = new();
        public List<VolunteerDirectoryItemVm> Items { get; set; } = new();
    }
}
