using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Customer;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
   
    public class CustomerReceiptConfiguration : IEntityTypeConfiguration<CustomerReceipt>
    {
        public void Configure(EntityTypeBuilder<CustomerReceipt> builder)
        {
            builder.ToTable("CustomerReceipts");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReceiptNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.Amount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.ReceiptNumber).IsUnique();

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
