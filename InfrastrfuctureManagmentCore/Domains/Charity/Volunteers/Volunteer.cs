namespace InfrastrfuctureManagmentCore.Domains.Charity.Volunteers
{
    public class Volunteer
    {
        public Guid Id { get; set; }
        public string VolunteerCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Qualification { get; set; }
        public string? AddressLine { get; set; }
        public string? Nationality { get; set; }
        public string? NationalId { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Gender { get; set; }      // Male / Female
        public string? MaritalStatus { get; set; } // Single / Married / Other
        public string? PreferredArea { get; set; }
        public string? AvailabilityNotes { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<VolunteerProjectAssignment> Assignments { get; set; } = new List<VolunteerProjectAssignment>();
        public ICollection<VolunteerHourLog> HourLogs { get; set; } = new List<VolunteerHourLog>();
    }
}
