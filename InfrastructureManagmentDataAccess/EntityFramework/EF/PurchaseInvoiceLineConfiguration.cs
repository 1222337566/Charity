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

    public class PurchaseInvoiceLineConfiguration : IEntityTypeConfiguration<PurchaseInvoiceLine>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoiceLine> builder)
        {
            builder.ToTable("PurchaseInvoiceLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.PurchaseInvoice)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
