using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class DonationAllocationConfiguration : IEntityTypeConfiguration<DonationAllocation>
    {
        public void Configure(EntityTypeBuilder<DonationAllocation> builder)
        {
            builder.ToTable("CharityDonationAllocations");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AllocatedQuantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);

            builder.HasIndex(x => x.DonationId);
            builder.HasIndex(x => x.BeneficiaryId);
            builder.HasIndex(x => x.DonationInKindItemId);
            builder.HasIndex(x => x.AllocatedDate);

            builder.HasOne(x => x.Donation)
                .WithMany()
                .HasForeignKey(x => x.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Beneficiary)
                .WithMany()
                .HasForeignKey(x => x.BeneficiaryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DonationInKindItem)
                .WithMany()
                .HasForeignKey(x => x.DonationInKindItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
