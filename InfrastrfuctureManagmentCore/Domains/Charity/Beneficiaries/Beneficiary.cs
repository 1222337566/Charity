using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;

namespace InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries
{
    public class Beneficiary
    {
        public Guid Id { get; set; }

        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }

        public Guid? GenderId { get; set; }
        public GenderLookup? Gender { get; set; }

        public Guid? MaritalStatusId { get; set; }
        public MaritalStatusLookup? MaritalStatus { get; set; }

        public string? PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }

        public string? AddressLine { get; set; }

        public Guid? GovernorateId { get; set; }
        public Governorate? Governorate { get; set; }

        public Guid? CityId { get; set; }
        public City? City { get; set; }

        public Guid? AreaId { get; set; }
        public Area? Area { get; set; }

        public int FamilyMembersCount { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? IncomeSource { get; set; }

        public string? HealthStatus { get; set; }
        public string? EducationStatus { get; set; }
        public string? WorkStatus { get; set; }
        public string? HousingStatus { get; set; }

        public string? Notes { get; set; }

        public Guid? StatusId { get; set; }
        public BeneficiaryStatusLookup? Status { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow.Date;
        public string? CreatedByUserId { get; set; }

        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<BeneficiaryFamilyMember> FamilyMembers { get; set; } = new List<BeneficiaryFamilyMember>();
        public ICollection<BeneficiaryDocument> Documents { get; set; } = new List<BeneficiaryDocument>();
        public ICollection<BeneficiaryAssessment> Assessments { get; set; } = new List<BeneficiaryAssessment>();
        public ICollection<BeneficiaryCommitteeDecision> CommitteeDecisions { get; set; } = new List<BeneficiaryCommitteeDecision>();
        public ICollection<BeneficiaryOldRecord> OldRecords { get; set; } = new List<BeneficiaryOldRecord>();
        public ICollection<BeneficiaryAidRequest> AidRequests { get; set; } = new List<BeneficiaryAidRequest>();
        public ICollection<BeneficiaryAidDisbursement> AidDisbursements { get; set; } = new List<BeneficiaryAidDisbursement>();
        public string? Location { get; set; }
    }
}
