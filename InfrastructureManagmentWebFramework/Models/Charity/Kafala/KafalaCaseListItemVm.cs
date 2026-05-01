namespace InfrastructureManagmentWebFramework.Models.Charity.Kafala
{
    public class KafalaCaseListItemVm
    {
        public Guid Id { get; set; }
        public string CaseNumber { get; set; } = string.Empty;
        public string SponsorName { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string SponsorshipType { get; set; } = string.Empty;
        public string Frequency { get; set; } = string.Empty;
        public decimal MonthlyAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public DateTime? LastCollectionDate { get; set; }
        public DateTime? LastDisbursementDate { get; set; }
        public bool AutoIncludeInAidCycles { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
