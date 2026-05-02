namespace InfrastructureManagmentWebFramework.Models.Charity.Volunteers
{
    public class VolunteerAssignmentListItemVm
    {
        public Guid Id { get; set; }
        public Guid VolunteerId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string RoleTitle { get; set; } = string.Empty;
        public string AssignmentType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? TargetHours { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
