namespace InfrastrfuctureManagmentCore.Domains.Charity.Volunteers
{
    public class VolunteerSkill
    {
        public Guid Id { get; set; }
        public Guid VolunteerId { get; set; }
        public Guid SkillDefinitionId { get; set; }
        public string SkillLevel { get; set; } = "Beginner"; // Beginner / Intermediate / Advanced
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public Volunteer? Volunteer { get; set; }
        public VolunteerSkillDefinition? SkillDefinition { get; set; }
    }
}
