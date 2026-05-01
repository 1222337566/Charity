using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchFamilyMemberConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchFamilyMember>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchFamilyMember> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchFamilyMembers");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FullName).HasMaxLength(250).IsRequired();
        }
    }
}
