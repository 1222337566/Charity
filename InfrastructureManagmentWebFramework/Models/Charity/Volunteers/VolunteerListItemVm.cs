namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerListItemVm
    {
        public Guid Id { get; set; }
        public string VolunteerCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string? PreferredArea { get; set; }
        public bool IsActive { get; set; }
        public int AssignmentsCount { get; set; }
        public decimal TotalHours { get; set; }
    }
}
