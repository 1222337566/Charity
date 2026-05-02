using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class CreateVolunteerAvailabilitySlotVm
    {
        [Required]
        public Guid VolunteerId { get; set; }
        [Required]
        public string DayOfWeekName { get; set; } = string.Empty;
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        [Required]
        public string AvailabilityType { get; set; } = "Regular";
        public string? Area { get; set; }
        public bool IsRemoteAllowed { get; set; }
        public string? Notes { get; set; }
        public List<SelectListItem> Days { get; set; } = new();
        public List<SelectListItem> Types { get; set; } = new();
    }
}
