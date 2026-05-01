using InfrastrfuctureManagmentCore.Domains.Charity.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CharityStoreReceiptConfiguration : IEntityTypeConfiguration<CharityStoreReceipt>
    {
        public void Configure(EntityTypeBuilder<CharityStoreReceipt> builder)
        {
            builder.ToTable("CharityStoreReceipts");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceiptNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.SourceType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.SourceName).HasMaxLength(200);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);

            builder.HasIndex(x => x.ReceiptNumber).IsUnique();
            builder.HasIndex(x => new { x.WarehouseId, x.ReceiptDate });

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(x => x.Lines)
                .WithOne(x => x.Receipt)
                .HasForeignKey(x => x.ReceiptId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
