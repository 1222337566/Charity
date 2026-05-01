using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
   
    public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
    {
        public void Configure(EntityTypeBuilder<Warehouse> builder)
        {
            builder.ToTable("Warehouses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WarehouseCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(x => x.WarehouseNameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.WarehouseNameEn)
                .HasMaxLength(200);

            builder.Property(x => x.Address)
                .HasMaxLength(500);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.WarehouseCode).IsUnique();
        }
    }
}
