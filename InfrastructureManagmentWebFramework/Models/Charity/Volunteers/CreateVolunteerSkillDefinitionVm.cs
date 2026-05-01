using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class CreateVolunteerSkillDefinitionVm
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
