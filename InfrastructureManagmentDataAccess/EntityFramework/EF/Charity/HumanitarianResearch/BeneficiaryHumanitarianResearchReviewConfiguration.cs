using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchReviewConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearchReview>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearchReview> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchReviews");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Decision).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Reason).HasMaxLength(2000).IsRequired();
        }
    }
}
