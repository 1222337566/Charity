using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchFamilyMember
    {
        public Guid Id { get; set; }
        public Guid ResearchId { get; set; }

        public string FullName { get; set; } = string.Empty;
        public string? Relationship { get; set; }
        public int? Age { get; set; }
        public string? EducationLevel { get; set; }
        public string? WorkType { get; set; }
        public decimal? Income { get; set; }
        public string? MaritalStatus { get; set; }
        public string? NationalId { get; set; }

        public BeneficiaryHumanitarianResearch? Research { get; set; }
    }
}
