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
  
    public class CustomerAccountTransactionConfiguration : IEntityTypeConfiguration<CustomerAccountTransaction>
    {
        public void Configure(EntityTypeBuilder<CustomerAccountTransaction> builder)
        {
            builder.ToTable("CustomerAccountTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ReferenceType)
                .HasMaxLength(100);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(500);

            builder.Property(x => x.DebitAmount)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.CreditAmount)
                .HasColumnType("decimal(18,2)");

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
