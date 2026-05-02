namespace InfrastrfuctureManagmentCore.Domains.Charity.Volunteers
{
    public class VolunteerAvailabilitySlot
    {
        public Guid Id { get; set; }
        public Guid VolunteerId { get; set; }
        public string DayOfWeekName { get; set; } = string.Empty;
        public TimeSpan? FromTime { get; set; }
        public TimeSpan? ToTime { get; set; }
        public string AvailabilityType { get; set; } = "Regular"; // Regular / Occasional / Emergency
        public string? Area { get; set; }
        public bool IsRemoteAllowed { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Volunteer? Volunteer { get; set; }
    }
}
