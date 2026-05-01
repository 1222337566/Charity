namespace InfrastrfuctureManagmentCore.Domains.Charity.Volunteers
{
    public class VolunteerSkillDefinition
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
