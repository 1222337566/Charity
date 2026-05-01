using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearch
    {
        public Guid Id { get; set; }
        public Guid BeneficiaryId { get; set; }
        public Beneficiary? Beneficiary { get; set; }

        public string ResearchNumber { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public DateTime? ResearchDate { get; set; }

        public string? AidTypeName { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string? SourceOfRequest { get; set; }

        public string? ResearcherCode { get; set; }
        public string? ResearcherName { get; set; }
        public string? CommitteeCode { get; set; }
        public string? PriorityLevel { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public int? Age { get; set; }
        public string? MaritalStatus { get; set; }
        public string? NationalId { get; set; }
        public string? AddressLine { get; set; }
        public string? Phone1 { get; set; }
        public string? Phone2 { get; set; }
        public string? Phone3 { get; set; }

        public int FamilyMembersCount { get; set; }
        public decimal? TotalIncome { get; set; }
        public decimal? TotalExpenses { get; set; }
        public decimal? AverageIncome { get; set; }

        public bool HasExistingProject { get; set; }
        public string? ExistingProjectType { get; set; }
        public string? ExistingProjectSize { get; set; }

        public string? RequiredNeedsPrimary { get; set; }
        public string? RequiredNeedsSecondary { get; set; }

        public string? HousingDescription { get; set; }
        public string? ResearcherReport { get; set; }
        public string? ResearchManagerOpinion { get; set; }

        public string Status { get; set; } = "Draft";
        public DateTime? SubmittedAtUtc { get; set; }
        public string? SubmittedByUserId { get; set; }

        public DateTime? ReviewedAtUtc { get; set; }
        public string? ReviewedByUserId { get; set; }
        public string? ReviewDecision { get; set; }
        public string? ReviewReason { get; set; }

        public DateTime? SentToCommitteeAtUtc { get; set; }
        public DateTime? CommitteeDecidedAtUtc { get; set; }

        public bool IsActive { get; set; } = true;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }

        public ICollection<BeneficiaryHumanitarianResearchFamilyMember> FamilyMembers { get; set; } = new List<BeneficiaryHumanitarianResearchFamilyMember>();
        public ICollection<BeneficiaryHumanitarianResearchIncomeItem> IncomeItems { get; set; } = new List<BeneficiaryHumanitarianResearchIncomeItem>();
        public ICollection<BeneficiaryHumanitarianResearchExpenseItem> ExpenseItems { get; set; } = new List<BeneficiaryHumanitarianResearchExpenseItem>();
        public ICollection<BeneficiaryHumanitarianResearchDebt> Debts { get; set; } = new List<BeneficiaryHumanitarianResearchDebt>();
        public ICollection<BeneficiaryHumanitarianResearchHouseAsset> HouseAssets { get; set; } = new List<BeneficiaryHumanitarianResearchHouseAsset>();
        public ICollection<BeneficiaryHumanitarianResearchReview> Reviews { get; set; } = new List<BeneficiaryHumanitarianResearchReview>();
        public ICollection<BeneficiaryHumanitarianResearchCommitteeEvaluation> CommitteeEvaluations { get; set; } = new List<BeneficiaryHumanitarianResearchCommitteeEvaluation>();
        public DateTime? CommitteeSentAtUtc { get; set; }
        public string? CommitteeSentByUserId { get; set; }
        public string? CommitteeDecisionByUserId { get; set; }
        public DateTime? CommitteeDecisionAtUtc { get; set; }
        public string? CommitteeDecision { get; set; }
        public string? CommitteeDecisionNotes { get; set; }
    }
}
