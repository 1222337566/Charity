namespace InfrastructureManagmentWebFramework.Models.AccountingIntegration
{
    public class AccountingIntegrationProfileListItemVm
    {
        public Guid Id { get; set; }
        public string SourceType { get; set; } = string.Empty;
        public string? SourceTypeNameAr { get; set; }
        public string? EventCode { get; set; }
        public string? EventNameAr { get; set; }
        public string ProfileNameAr { get; set; } = string.Empty;
        public string? MatchDonationType { get; set; }
        public string? MatchTargetingScopeCode { get; set; }
        public string? MatchPurposeName { get; set; }
        public string? MatchAidTypeNameAr { get; set; }
        public string? MatchStoreMovementType { get; set; }
        public int Priority { get; set; }
        public string? DebitAccountCode { get; set; }
        public string? DebitAccountName { get; set; }
        public string? CreditAccountCode { get; set; }
        public string? CreditAccountName { get; set; }
        public string? CostCenterNameAr { get; set; }
        public bool UseSourceFinancialAccountAsDebit { get; set; }
        public bool UseSourceFinancialAccountAsCredit { get; set; }
        public bool AutoPost { get; set; }
        public bool IsActive { get; set; }
    }
}
