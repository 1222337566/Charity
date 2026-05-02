using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class EditVolunteerSkillVm : CreateVolunteerSkillVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
