using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchHouseAssetConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchHouseAsset>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchHouseAsset> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchHouseAssets");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.AssetCategory).HasMaxLength(100).IsRequired();
            builder.Property(x => x.AssetName).HasMaxLength(150).IsRequired();
        }
    }
}
