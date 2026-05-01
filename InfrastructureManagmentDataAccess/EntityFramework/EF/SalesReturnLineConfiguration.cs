using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.SalesReturn;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
  
    public class SalesReturnLineConfiguration : IEntityTypeConfiguration<SalesReturnLine>
    {
        public void Configure(EntityTypeBuilder<SalesReturnLine> builder)
        {
            builder.ToTable("SalesReturnLines");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity).HasColumnType("decimal(18,2)");
            builder.Property(x => x.UnitPrice).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.LineTotal).HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.SalesReturnInvoice)
                .WithMany(x => x.Lines)
                .HasForeignKey(x => x.SalesReturnInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.OriginalSalesInvoiceLine)
                .WithMany()
                .HasForeignKey(x => x.OriginalSalesInvoiceLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
