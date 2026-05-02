using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using InfrastrfuctureManagmentCore.Domains.Charity.Projects;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryAidRequest
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public DateTime RequestDate { get; set; }
        public Guid AidTypeId { get; set; }
        public AidTypeLookup? AidType { get; set; }

        public decimal? RequestedAmount { get; set; }
        public string? Reason { get; set; }
        public string? UrgencyLevel { get; set; }
        public string Status { get; set; } = "Pending";

        public Guid? ProjectId { get; set; }
        public CharityProject project { get; set; }

        public ICollection<BeneficiaryAidRequestLine> Lines { get; set; } = new List<BeneficiaryAidRequestLine>();
        public string? CreatedByUserId { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
