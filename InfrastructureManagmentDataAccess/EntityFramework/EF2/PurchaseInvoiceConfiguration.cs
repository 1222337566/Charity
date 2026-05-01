using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Purchase;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class PurchaseInvoiceConfiguration : IEntityTypeConfiguration<PurchaseInvoice>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
        {
            builder.ToTable("PurchaseInvoices");

            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Supplier)
                    .WithMany()
                    .HasForeignKey(x => x.SupplierId)
                    .OnDelete(DeleteBehavior.Restrict);
            builder.Property(x => x.InvoiceNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.SupplierName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.SupplierInvoiceNumber)
                .HasMaxLength(100);

            builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.NetAmount).HasColumnType("decimal(18,2)");

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.InvoiceNumber).IsUnique();

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
