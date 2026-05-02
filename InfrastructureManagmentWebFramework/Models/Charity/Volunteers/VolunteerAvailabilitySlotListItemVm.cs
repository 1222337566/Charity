namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerAvailabilitySlotListItemVm
    {
        public Guid Id { get; set; }
        public string DayOfWeekName { get; set; } = string.Empty;
        public string? TimeText { get; set; }
        public string AvailabilityType { get; set; } = string.Empty;
        public string? Area { get; set; }
        public bool IsRemoteAllowed { get; set; }
        public string? Notes { get; set; }
    }
}
