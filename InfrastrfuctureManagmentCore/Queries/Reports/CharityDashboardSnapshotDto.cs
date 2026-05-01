namespace InfrastrfuctureManagmentCore.Queries.Reports
{
    public class CharityDashboardSnapshotDto
    {
        public int BeneficiariesCount { get; set; }
        public int DonorsCount { get; set; }
        public int FundersCount { get; set; }
        public int ProjectsCount { get; set; }
        public int EmployeesCount { get; set; }
        public decimal TotalDonations { get; set; }
        public decimal TotalReceivedGrants { get; set; }
        public decimal TotalAidDisbursed { get; set; }
        public decimal TotalPayrollNet { get; set; }

        public List<StatusCountDto> BeneficiaryStatuses { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyDonations { get; set; } = new();
        public List<ProjectOverviewDto> TopProjects { get; set; } = new();
        public List<PayrollMonthOverviewDto> RecentPayrollMonths { get; set; } = new();
        public List<StoreMovementOverviewDto> StoreMovements { get; set; } = new();

        // ── Kafala ──
        public int KafalaCasesTotal { get; set; }
        public int KafalaCasesActive { get; set; }
        public int KafalaCasesSuspended { get; set; }
        public int KafalaCasesClosed { get; set; }
        public int KafalaSponsorsCount { get; set; }
        public decimal KafalaTotalCollected { get; set; }
        public decimal KafalaTotalDisbursed { get; set; }
        public List<StatusCountDto> KafalaBySponsorshipType { get; set; } = new();
        public List<StatusCountDto> KafalaByFrequency { get; set; } = new();
        public List<MonthlyAmountDto> KafalaMonthlyCollected { get; set; } = new();
        public List<MonthlyAmountDto> KafalaMonthlyDisbursed { get; set; } = new();

        // ── Accounting ──
        public decimal TotalAssets { get; set; }
        public decimal TotalLiabilities { get; set; }
        public decimal TotalEquity { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal NetProfit => TotalRevenue - TotalExpenses;
        public List<StatusCountDto> AccountsByCategory { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyRevenue { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyExpenses { get; set; } = new();

        // ── Historical — Beneficiaries ──
        public List<MonthlyAmountDto> MonthlyNewBeneficiaries { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyAidRequests { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyAidDisbursements { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyAidDisbursedAmount { get; set; } = new();

        // ── Historical — Donors ──
        public List<MonthlyAmountDto> MonthlyNewDonors { get; set; } = new();

        // ── Historical — Funders ──
        public List<MonthlyAmountDto> MonthlyGrantsReceived { get; set; } = new();
        public List<StatusCountDto> GrantAgreementsByStatus { get; set; } = new();

        // ── Historical — Projects ──
        public List<MonthlyAmountDto> MonthlyNewProjects { get; set; } = new();
        public List<StatusCountDto> ProjectsByStatus { get; set; } = new();
        public List<MonthlyAmountDto> MonthlyProjectBudget { get; set; } = new();
    }
}
