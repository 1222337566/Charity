using InfrastrfuctureManagmentCore.Domains.Warehouses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    
    

public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
    {
        public void Configure(EntityTypeBuilder<StockTransaction> builder)
        {
            builder.ToTable("StockTransactions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Quantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.UnitCost)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ReferenceType)
                .HasMaxLength(100);

            builder.Property(x => x.ReferenceNumber)
                .HasMaxLength(100);

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.RelatedWarehouse)
                .WithMany()
                .HasForeignKey(x => x.RelatedWarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
