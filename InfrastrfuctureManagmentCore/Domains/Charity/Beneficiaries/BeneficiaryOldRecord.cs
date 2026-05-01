namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryOldRecord
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public DateTime RecordDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Details { get; set; }

        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
    }
}
