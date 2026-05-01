namespace InfrastructureManagmentWebFramework.Models.Charity.Beneficiaries
{
    public class BeneficiaryDetailsVm
    {
        public Guid Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? GenderText { get; set; }
        public string? MaritalStatusText { get; set; }
        public string? PhoneNumber { get; set; }
        public string? AlternatePhoneNumber { get; set; }
        public string? AddressLine { get; set; }
        public string? GovernorateName { get; set; }
        public string? CityName { get; set; }
        public string? AreaName { get; set; }
        public int FamilyMembersCount { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public string? IncomeSource { get; set; }
        public string? HealthStatus { get; set; }
        public string? EducationStatus { get; set; }
        public string? WorkStatus { get; set; }
        public string? HousingStatus { get; set; }
        public string? StatusText { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }

        public List<BeneficiaryFamilyMemberItemVm> FamilyMembers { get; set; } = new();
        public List<BeneficiaryDocumentItemVm> Documents { get; set; } = new();
        public List<BeneficiaryAssessmentItemVm> Assessments { get; set; } = new();
        public List<BeneficiaryCommitteeDecisionItemVm> CommitteeDecisions { get; set; } = new();
        public List<BeneficiaryAidRequestItemVm> AidRequests { get; set; } = new();
        public List<BeneficiaryAidDisbursementItemVm> AidDisbursements { get; set; } = new();
        public List<BeneficiaryOldRecordItemVm> OldRecords { get; set; } = new();
    }

    public class BeneficiaryFamilyMemberItemVm
    {
        public string FullName { get; set; } = string.Empty;
        public string? Relationship { get; set; }
        public string? NationalId { get; set; }
        public DateTime? BirthDate { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public bool IsDependent { get; set; }
    }

    public class BeneficiaryDocumentItemVm
    {
        public string DocumentType { get; set; } = string.Empty;
        public string? DocumentNumber { get; set; }
        public DateTime? IssueDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool IsVerified { get; set; }
    }

    public class BeneficiaryAssessmentItemVm
    {
        public DateTime VisitDate { get; set; }
        public string? RecommendedAidType { get; set; }
        public decimal? RecommendationAmount { get; set; }
        public decimal? AssessmentScore { get; set; }
        public string? RecommendationText { get; set; }
    }

    public class BeneficiaryCommitteeDecisionItemVm
    {
        public DateTime DecisionDate { get; set; }
        public string DecisionType { get; set; } = string.Empty;
        public string? ApprovedAidType { get; set; }
        public decimal? ApprovedAmount { get; set; }
        public int? DurationInMonths { get; set; }
        public string? CommitteeNotes { get; set; }
        public bool ApprovedStatus { get; set; }
    }

    public class BeneficiaryAidRequestItemVm
    {
        public DateTime RequestDate { get; set; }
        public string? AidType { get; set; }
        public decimal? RequestedAmount { get; set; }
        public string? Status { get; set; }
        public string? Reason { get; set; }
    }

    public class BeneficiaryAidDisbursementItemVm
    {
        public DateTime DisbursementDate { get; set; }
        public string? AidType { get; set; }
        public decimal? Amount { get; set; }
        public string? Notes { get; set; }
    }

    public class BeneficiaryOldRecordItemVm
    {
        public DateTime RecordDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Details { get; set; }
    }
}
