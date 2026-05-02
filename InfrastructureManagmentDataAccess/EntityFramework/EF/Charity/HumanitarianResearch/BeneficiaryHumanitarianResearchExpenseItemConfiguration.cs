using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchExpenseItemConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchExpenseItem>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchExpenseItem> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchExpenseItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ExpenseType).HasMaxLength(200).IsRequired();
        }
    }
}
