namespace InfrastructureManagmentWebFramework.Models.HR.Advanced.Evaluations
{
    public class PerformanceEvaluationListItemVm
    {
        public Guid Id { get; set; }
        public Guid EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string EvaluationPeriod { get; set; } = string.Empty;
        public DateTime EvaluationDate { get; set; }
        public decimal TotalScore { get; set; }
        public string Result { get; set; } = string.Empty;
        public string? EvaluatorName { get; set; }
    }
}
