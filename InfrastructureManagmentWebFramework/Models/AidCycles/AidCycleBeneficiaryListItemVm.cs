namespace InfrastructureManagmentWebFramework.Models.Charity.AidCycles
{
    public class AidCycleBeneficiaryListItemVm
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public string BeneficiaryCode { get; set; } = string.Empty;
        public string BeneficiaryName { get; set; } = string.Empty;
        public string? AidType { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? LastDisbursementDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string? AddedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public bool IsKafalaLinked { get; set; }
        public Guid? KafalaCaseId { get; set; }
        public string? KafalaCaseNumber { get; set; }
        public string? KafalaSponsorName { get; set; }
        public string? KafalaFrequency { get; set; }
        public string? ReferenceLabel { get; set; }
    }
}
