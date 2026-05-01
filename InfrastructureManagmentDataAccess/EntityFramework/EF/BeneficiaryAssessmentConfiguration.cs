using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryAssessmentConfiguration : IEntityTypeConfiguration<BeneficiaryAssessment>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryAssessment> builder)
        {
            builder.ToTable("CharityBeneficiaryAssessments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.HousingCondition).HasMaxLength(500);
            builder.Property(x => x.EconomicCondition).HasMaxLength(500);
            builder.Property(x => x.HealthCondition).HasMaxLength(500);
            builder.Property(x => x.SocialCondition).HasMaxLength(500);
            builder.Property(x => x.RecommendationAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.AssessmentScore).HasColumnType("decimal(10,2)");
            builder.Property(x => x.RecommendationText).HasMaxLength(2000);
            builder.Property(x => x.DecisionSuggested).HasMaxLength(250);

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.Assessments)
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.RecommendedAidType)
                .WithMany()
                .HasForeignKey(x => x.RecommendedAidTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ResearcherUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
