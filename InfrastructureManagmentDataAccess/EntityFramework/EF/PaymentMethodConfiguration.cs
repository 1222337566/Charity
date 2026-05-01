using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Payments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
  

    public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
    {
        public void Configure(EntityTypeBuilder<PaymentMethod> builder)
        {
            builder.ToTable("PaymentMethods");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.MethodCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.MethodNameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.MethodNameEn)
                .HasMaxLength(200);

            builder.HasIndex(x => x.MethodCode).IsUnique();
        }
    }
}
