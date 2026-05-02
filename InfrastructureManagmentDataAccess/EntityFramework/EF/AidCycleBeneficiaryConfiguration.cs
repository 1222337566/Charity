using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AidCycleBeneficiaryConfiguration : IEntityTypeConfiguration<AidCycleBeneficiary>
    {
        public void Configure(EntityTypeBuilder<AidCycleBeneficiary> builder)
        {
            builder.ToTable("CharityAidCycleBeneficiaries");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.StopReason).HasMaxLength(500);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.Property(x => x.ScheduledAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ApprovedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DisbursedAmount).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => new { x.AidCycleId, x.BeneficiaryId }).IsUnique();
            builder.HasIndex(x => x.Status);
            builder.HasIndex(x => x.NextDueDate);

            builder.HasOne(x => x.AidCycle)
                .WithMany(x => x.Beneficiaries)
                .HasForeignKey(x => x.AidCycleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Beneficiary)
                .WithMany()
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CommitteeDecision)
                .WithMany()
                .HasForeignKey(x => x.CommitteeDecisionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AidType)
                .WithMany()
                .HasForeignKey(x => x.AidTypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
