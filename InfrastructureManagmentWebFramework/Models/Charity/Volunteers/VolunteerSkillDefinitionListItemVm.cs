namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerSkillDefinitionListItemVm
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
