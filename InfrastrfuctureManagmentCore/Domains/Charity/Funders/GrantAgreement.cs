namespace InfrastrfuctureManagmentCore.Domains.Charity.Funders
{
    public class GrantAgreement
    {
        public Guid Id { get; set; }
        public string AgreementNumber { get; set; } = string.Empty;

        public Guid FunderId { get; set; }
        public Funder? Funder { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime AgreementDate { get; set; } = DateTime.Today;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = "EGP";
        public string? PaymentTerms { get; set; }
        public string? ReportingRequirements { get; set; }
        public string Status { get; set; } = "Draft";
        public string? Notes { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<GrantInstallment> Installments { get; set; } = new List<GrantInstallment>();
        public ICollection<GrantCondition> Conditions { get; set; } = new List<GrantCondition>();
    }
}
