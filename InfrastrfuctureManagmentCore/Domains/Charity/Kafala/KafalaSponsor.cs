namespace InfrastrfuctureManagmentCore.Domains.Charity.Kafala
{
    public class KafalaSponsor
    {
        public Guid Id { get; set; }
        public string SponsorCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string SponsorType { get; set; } = "Individual"; // Individual / Company / Institution
        public string? NationalIdOrTaxNo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? AddressLine { get; set; }
        public bool IsActive { get; set; } = true;
        public string? Notes { get; set; }
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<KafalaCase> KafalaCases { get; set; } = new List<KafalaCase>();
        public ICollection<KafalaPayment> Payments { get; set; } = new List<KafalaPayment>();
    }
}
