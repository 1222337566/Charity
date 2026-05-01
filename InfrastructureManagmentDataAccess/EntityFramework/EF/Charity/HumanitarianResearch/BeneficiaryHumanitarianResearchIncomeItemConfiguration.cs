using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchIncomeItemConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchIncomeItem>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchIncomeItem> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchIncomeItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.IncomeType).HasMaxLength(200).IsRequired();
        }
    }
}
