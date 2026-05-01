using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Sale;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SalesInvoiceLineConfiguration : IEntityTypeConfiguration<SalesInvoiceLine>
    {
        public void Configure(EntityTypeBuilder<SalesInvoiceLine> builder)
        {
            builder.ToTable("SalesInvoiceLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.SalesInvoice)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
