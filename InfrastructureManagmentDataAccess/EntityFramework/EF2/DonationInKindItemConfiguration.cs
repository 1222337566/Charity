using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class DonationInKindItemConfiguration : IEntityTypeConfiguration<DonationInKindItem>
    {
        public void Configure(EntityTypeBuilder<DonationInKindItem> builder)
        {
            builder.ToTable("CharityDonationInKindItems");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.EstimatedUnitValue).HasColumnType("decimal(18,2)");
            builder.Property(x => x.EstimatedTotalValue).HasColumnType("decimal(18,2)");
            builder.Property(x => x.BatchNo).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(1000);

            builder.HasIndex(x => x.DonationId);
            builder.HasIndex(x => x.ItemId);
            builder.HasIndex(x => x.WarehouseId);

            builder.HasOne(x => x.Donation)
                .WithMany()
                .HasForeignKey(x => x.DonationId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
