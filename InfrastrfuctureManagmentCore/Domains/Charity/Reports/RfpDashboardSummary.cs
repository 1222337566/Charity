namespace InfrastrfuctureManagmentCore.Domains.Charity.Reports
{
    public class RfpDashboardSummary
    {
        public int EmployeesCount { get; set; }
        public int ActiveEmployeesCount { get; set; }
        public int BeneficiariesCount { get; set; }
        public int ApprovedBeneficiariesCount { get; set; }
        public int MonthlyAidDisbursementsCount { get; set; }
        public decimal MonthlyAidTotalAmount { get; set; }
        public int ProjectsCount { get; set; }
        public int ActiveProjectsCount { get; set; }
        public int BoardMeetingsCount { get; set; }
        public int BoardDecisionsCount { get; set; }
    }
}
