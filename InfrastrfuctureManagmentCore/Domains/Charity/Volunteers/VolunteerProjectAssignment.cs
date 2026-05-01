namespace InfrastrfuctureManagmentCore.Domains.Charity.Volunteers
{
    public class VolunteerProjectAssignment
    {
        public Guid Id { get; set; }

        public Guid VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }

        public Guid? ProjectId { get; set; }  // CharityProject
        public string? ProjectNameSnapshot { get; set; }

        public string RoleTitle { get; set; } = string.Empty;
        public string AssignmentType { get; set; } = "Recurring"; // Recurring / Event / Seasonal
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TargetHours { get; set; }
        public string Status { get; set; } = "Active"; // Active / Completed / Paused / Cancelled
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<VolunteerHourLog> HourLogs { get; set; } = new List<VolunteerHourLog>();
    }
}
