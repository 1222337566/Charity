using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Item;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ItemConfiguration : IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Items");

            builder.HasKey(x => x.Id);
            builder.Property(x => x.BrandName)
    .HasMaxLength(100);

            builder.Property(x => x.ModelName)
                .HasMaxLength(100);

            builder.Property(x => x.Color)
                .HasMaxLength(100);

            builder.Property(x => x.EyeSize)
                .HasColumnType("decimal(8,2)");

            builder.Property(x => x.BridgeSize)
                .HasColumnType("decimal(8,2)");

            builder.Property(x => x.TempleLength)
                .HasColumnType("decimal(8,2)");

            builder.Property(x => x.LensMaterial)
                .HasMaxLength(100);

            builder.Property(x => x.LensIndex)
                .HasMaxLength(50);

            builder.Property(x => x.LensCoating)
                .HasMaxLength(100);
            builder.Property(x => x.ItemCode)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.ItemNameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.ItemNameEn)
                .HasMaxLength(200);

            builder.Property(x => x.Barcode)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.PurchasePrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.SalePrice)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.MinimumQuantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ReorderQuantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.TaxRate)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.ItemCode).IsUnique();
            builder.HasIndex(x => x.Barcode).IsUnique()
                .HasFilter("[Barcode] IS NOT NULL");

            builder.HasOne(x => x.ItemGroup)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.ItemGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Unit)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.UnitId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
