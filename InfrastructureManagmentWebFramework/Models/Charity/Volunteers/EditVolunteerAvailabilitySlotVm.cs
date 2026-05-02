using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class EditVolunteerAvailabilitySlotVm : CreateVolunteerAvailabilitySlotVm
    {
        [Required]
        public Guid Id { get; set; }
    }
}
