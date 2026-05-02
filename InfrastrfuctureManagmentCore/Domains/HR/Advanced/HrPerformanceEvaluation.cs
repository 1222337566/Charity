using InfrastrfuctureManagmentCore.Domains.HR;

namespace InfrastrfuctureManagmentCore.Domains.HR.Advanced
{
    public class HrPerformanceEvaluation
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public HrEmployee? Employee { get; set; }
        public Guid? EvaluatorEmployeeId { get; set; }
        public HrEmployee? EvaluatorEmployee { get; set; }
        public string EvaluationPeriod { get; set; } = string.Empty;
        public DateTime EvaluationDate { get; set; } = DateTime.Today;
        public decimal DisciplineScore { get; set; }
        public decimal PerformanceScore { get; set; }
        public decimal CooperationScore { get; set; }
        public decimal InitiativeScore { get; set; }
        public decimal TotalScore { get; set; }
        public string Result { get; set; } = "Good";
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
