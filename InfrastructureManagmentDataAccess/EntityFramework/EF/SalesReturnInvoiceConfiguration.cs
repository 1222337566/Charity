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
  
    public class SalesReturnInvoiceConfiguration : IEntityTypeConfiguration<SalesReturnInvoice>
    {
        public void Configure(EntityTypeBuilder<SalesReturnInvoice> builder)
        {
            builder.ToTable("SalesReturnInvoices");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReturnNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.CustomerName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.SubTotal).HasColumnType("decimal(18,2)");
            builder.Property(x => x.DiscountAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TaxAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.NetAmount).HasColumnType("decimal(18,2)");

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.ReturnNumber).IsUnique();

            builder.HasOne(x => x.OriginalSalesInvoice)
                .WithMany()
                .HasForeignKey(x => x.OriginalSalesInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
