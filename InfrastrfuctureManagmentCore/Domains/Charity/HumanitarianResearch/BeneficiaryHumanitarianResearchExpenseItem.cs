using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchExpenseItem
    {
        public Guid Id { get; set; }
        public Guid ResearchId { get; set; }
        public string ExpenseType { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Notes { get; set; }

        public BeneficiaryHumanitarianResearch? Research { get; set; }
    }
}
