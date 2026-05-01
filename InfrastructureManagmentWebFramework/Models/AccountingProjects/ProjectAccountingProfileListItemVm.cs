namespace InfrastructureManagmentWebFramework.Models.AccountingProjects
{
    public class ProjectAccountingProfileListItemVm
    {
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? CostCenterName { get; set; }
        public string? RevenueAccountName { get; set; }
        public string? ExpenseAccountName { get; set; }
        public bool AutoTagJournalLinesWithProject { get; set; }
        public bool AutoUseProjectCostCenter { get; set; }
        public bool IsActive { get; set; }
    }
}
