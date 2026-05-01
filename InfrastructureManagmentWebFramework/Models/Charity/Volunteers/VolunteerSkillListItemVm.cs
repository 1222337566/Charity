namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerSkillListItemVm
    {
        public Guid Id { get; set; }
        public string SkillName { get; set; } = string.Empty;
        public string? Category { get; set; }
        public string SkillLevel { get; set; } = string.Empty;
        public string? Notes { get; set; }
    }
}
