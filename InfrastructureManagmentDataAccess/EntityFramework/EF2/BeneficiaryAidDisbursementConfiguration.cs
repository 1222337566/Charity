using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryAidDisbursementConfiguration : IEntityTypeConfiguration<BeneficiaryAidDisbursement>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryAidDisbursement> builder)
        {
            builder.ToTable("CharityAidDisbursements");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ExecutedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.ApprovalStatus).HasMaxLength(50).HasDefaultValue("Approved");
            builder.Property(x => x.ExecutionStatus).HasMaxLength(50).HasDefaultValue("FullyDisbursed");
            builder.Property(x => x.SourceType).HasMaxLength(100);
            builder.HasIndex(x => x.AidRequestLineId);

            builder.HasOne(x => x.AidRequestLine)
                .WithMany()
                .HasForeignKey(x => x.AidRequestLineId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasIndex(x => x.ApprovalStatus);
            builder.HasIndex(x => x.ExecutionStatus);
            builder.HasIndex(x => new { x.SourceType, x.SourceId });

            builder.HasOne(x => x.Beneficiary)
                .WithMany(x => x.AidDisbursements)
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.AidRequest)
                .WithMany()
                .HasForeignKey(x => x.AidRequestId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.AidType)
                .WithMany()
                .HasForeignKey(x => x.AidTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.FinancialAccount)
                .WithMany()
                .HasForeignKey(x => x.FinancialAccountId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ApprovedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.RejectedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ExecutedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
