using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryAidDisbursementFundingLineConfiguration : IEntityTypeConfiguration<BeneficiaryAidDisbursementFundingLine>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryAidDisbursementFundingLine> builder)
        {
            builder.ToTable("CharityAidDisbursementFundingLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.AmountConsumed).HasColumnType("decimal(18,2)");
            builder.HasIndex(x => x.DisbursementId);
            builder.HasIndex(x => x.DonationAllocationId);

            builder.HasOne(x => x.Disbursement)
                .WithMany(x => x.FundingLines)
                .HasForeignKey(x => x.DisbursementId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.DonationAllocation)
                .WithMany(x => x.DisbursementFundingLines)
                .HasForeignKey(x => x.DonationAllocationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
