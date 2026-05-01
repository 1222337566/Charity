namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerHourLogListItemVm
    {
        public Guid Id { get; set; }
        public Guid VolunteerId { get; set; }
        public DateTime WorkDate { get; set; }
        public decimal Hours { get; set; }
        public string ActivityTitle { get; set; } = string.Empty;
        public string? ProjectName { get; set; }
        public string? Outcome { get; set; }
    }
}
