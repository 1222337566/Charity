namespace InfrastrfuctureManagmentCore.Domains.Charity.Funders
{
    public class GrantCondition
    {
        public Guid Id { get; set; }

        public Guid GrantAgreementId { get; set; }
        public GrantAgreement? GrantAgreement { get; set; }

        public string ConditionTitle { get; set; } = string.Empty;
        public string ConditionDetails { get; set; } = string.Empty;
        public bool IsMandatory { get; set; }
        public DateTime? DueDate { get; set; }
        public bool IsFulfilled { get; set; }
        public DateTime? FulfilledDate { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
