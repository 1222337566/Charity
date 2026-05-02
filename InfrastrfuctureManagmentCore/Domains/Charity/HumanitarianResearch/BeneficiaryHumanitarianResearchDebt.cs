using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchDebt
    {
        public Guid Id { get; set; }
        public Guid ResearchId { get; set; }
        public string? DebtType { get; set; }
        public decimal? Amount { get; set; }
        public string? Reason { get; set; }
        public bool HasLegalCase { get; set; }

        public BeneficiaryHumanitarianResearch? Research { get; set; }
    }
}
