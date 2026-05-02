using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Suppliers;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
    {
        public void Configure(EntityTypeBuilder<Supplier> builder)
        {
            builder.ToTable("Suppliers");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.SupplierNumber)
                .IsRequired()
                .HasMaxLength(30);

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.NameEn)
                .HasMaxLength(200);

            builder.Property(x => x.ContactPerson)
                .HasMaxLength(200);

            builder.Property(x => x.Tel)
                .HasMaxLength(50);

            builder.Property(x => x.MobileNo)
                .HasMaxLength(50);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.TaxNumber)
                .HasMaxLength(100);

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.SupplierNumber).IsUnique();
        }
    }
}
