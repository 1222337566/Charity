using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryCommitteeDecisionConfiguration : IEntityTypeConfiguration<BeneficiaryCommitteeDecision>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryCommitteeDecision> builder)
        {
            builder.ToTable("CharityBeneficiaryCommitteeDecisions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DecisionType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.ApprovedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.CommitteeNotes).HasMaxLength(2000);

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.CommitteeDecisions)
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ApprovedAidType)
                .WithMany()
                .HasForeignKey(x => x.ApprovedAidTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ApprovedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
