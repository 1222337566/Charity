using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Sale;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
  

    public class SalesInvoicePaymentConfiguration : IEntityTypeConfiguration<SalesInvoicePayment>
    {
        public void Configure(EntityTypeBuilder<SalesInvoicePayment> builder)
        {
            builder.ToTable("SalesInvoicePayments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.SalesInvoice)
                .WithMany(x => x.Payments)
                .HasForeignKey(x => x.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
