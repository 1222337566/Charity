using InfrastrfuctureManagmentCore.Domains.Charity.HumanitarianResearch;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.HumanitarianResearch
{
    public class BeneficiaryHumanitarianResearchConfiguration : IEntityTypeConfiguration<BeneficiaryHumanitarianResearch>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryHumanitarianResearch> builder)
        {
            builder.ToTable("CharityBeneficiaryHumanitarianResearchs");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ResearchNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ApplicantName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ResearchManagerOpinion).HasMaxLength(2000);
            builder.Property(x => x.ResearcherReport).HasMaxLength(4000);

            builder.HasIndex(x => x.ResearchNumber).IsUnique();

            builder.HasOne(x => x.Beneficiary)
                   .WithMany()
                   .HasForeignKey(x => x.BeneficiaryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.FamilyMembers)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.IncomeItems)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.ExpenseItems)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Debts)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.HouseAssets)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.Reviews)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x => x.CommitteeEvaluations)
                   .WithOne(x => x.Research)
                   .HasForeignKey(x => x.ResearchId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
