namespace InfrastrfuctureManagmentCore.Domains.Charity.Volunteers
{
    public class VolunteerHourLog
    {
        public Guid Id { get; set; }

        public Guid VolunteerId { get; set; }
        public Volunteer? Volunteer { get; set; }

        public Guid? AssignmentId { get; set; }
        public VolunteerProjectAssignment? Assignment { get; set; }

        public Guid? ProjectId { get; set; } // CharityProject
        public string? ProjectNameSnapshot { get; set; }

        public DateTime WorkDate { get; set; }
        public decimal Hours { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public string? Outcome { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
