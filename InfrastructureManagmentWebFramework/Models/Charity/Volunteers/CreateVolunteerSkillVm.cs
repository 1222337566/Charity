using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class CreateVolunteerSkillVm
    {
        [Required]
        public Guid VolunteerId { get; set; }
        [Required]
        public Guid SkillDefinitionId { get; set; }
        [Required]
        public string SkillLevel { get; set; } = "Beginner";
        public string? Notes { get; set; }
        public List<SelectListItem> SkillDefinitions { get; set; } = new();
        public List<SelectListItem> SkillLevels { get; set; } = new();
    }
}
