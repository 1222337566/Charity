using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchDebtConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchDebt>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchDebt> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchDebts");
            builder.HasKey(x => x.Id);
        }
    }
}
