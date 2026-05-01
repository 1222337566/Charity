using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class EditVolunteerSkillDefinitionVm : CreateVolunteerSkillDefinitionVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
