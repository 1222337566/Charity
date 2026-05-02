using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryAidRequestLineConfiguration : IEntityTypeConfiguration<BeneficiaryAidRequestLine>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryAidRequestLine> builder)
        {
            builder.ToTable("BeneficiaryAidRequestLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ItemNameSnapshot).HasMaxLength(250);
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.Property(x => x.RequestedQuantity).HasColumnType("decimal(18,3)");
            builder.Property(x => x.ApprovedQuantity).HasColumnType("decimal(18,3)");
            builder.Property(x => x.EstimatedUnitValue).HasColumnType("decimal(18,2)");
            builder.Property(x => x.EstimatedTotalValue).HasColumnType("decimal(18,2)");
            builder.Property(x => x.FulfillmentMethod)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("CashEquivalent");
            builder.Property(x => x.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasDefaultValue("Pending");
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.Property(x => x.UpdatedByUserId).HasMaxLength(450);

            builder.HasIndex(x => x.AidRequestId);
            builder.HasIndex(x => x.ItemId);
            builder.HasIndex(x => x.WarehouseId);
            builder.HasIndex(x => x.FulfillmentMethod);

            builder.HasOne(x => x.AidRequest)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.AidRequestId)
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
