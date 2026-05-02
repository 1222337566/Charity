using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Warehouses;
namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ItemWarehouseBalanceConfiguration : IEntityTypeConfiguration<ItemWarehouseBalance>
    {
        public void Configure(EntityTypeBuilder<ItemWarehouseBalance> builder)
        {
            builder.ToTable("ItemWarehouseBalances");

            

            builder.Property(x => x.QuantityOnHand)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.ReservedQuantity)
                .HasColumnType("decimal(18,2)");

            builder.Property(x => x.AvailableQuantity)
                .HasColumnType("decimal(18,2)");

            builder.HasIndex(x => new { x.ItemId, x.WarehouseId })
                .IsUnique();

            builder.HasOne(x => x.Item)
                .WithMany()
                .HasForeignKey(x => x.ItemId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
