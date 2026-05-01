using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class BeneficiaryFamilyMember
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string Relationship { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }

        public Guid? GenderId { get; set; }
        public GenderLookup? Gender { get; set; }

        public string? EducationStatus { get; set; }
        public string? WorkStatus { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? HealthCondition { get; set; }
        public bool IsDependent { get; set; } = true;
        public string? Notes { get; set; }
    }
}
