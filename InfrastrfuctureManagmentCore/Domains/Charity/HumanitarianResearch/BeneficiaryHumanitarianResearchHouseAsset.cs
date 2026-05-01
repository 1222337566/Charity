using System;

namespace InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchHouseAsset
    {
        public Guid Id { get; set; }
        public Guid ResearchId { get; set; }
        public string AssetCategory { get; set; } = string.Empty;
        public string AssetName { get; set; } = string.Empty;
        public bool Exists { get; set; }
        public string? Condition { get; set; }
        public int? Count { get; set; }
        public string? Notes { get; set; }

        public BeneficiaryHumanitarianResearch? Research { get; set; }
    }
}
