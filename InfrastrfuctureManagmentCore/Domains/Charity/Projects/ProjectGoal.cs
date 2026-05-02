namespace InfrastrfuctureManagmentCore.Domains.Charity.Projects
{
    /// <summary>الهدف الرئيسي للمشروع</summary>
    public class ProjectGoal
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public CharityProject? Project { get; set; }

        public int SortOrder { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? SuccessIndicator { get; set; }   // مؤشر قياس النجاح
        public string? TargetValue { get; set; }         // القيمة المستهدفة
        public string? AchievedValue { get; set; }       // القيمة المحققة
        public string Status { get; set; } = "Active";   // Active | Achieved | Cancelled
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public ICollection<ProjectSubGoal> SubGoals { get; set; } = new List<ProjectSubGoal>();
    }
}
