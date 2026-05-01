using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.PosHolds;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
   
    public class PosHoldConfiguration : IEntityTypeConfiguration<PosHold>
    {
        public void Configure(EntityTypeBuilder<PosHold> builder)
        {
            builder.ToTable("PosHolds");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.HoldNumber)
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

            builder.HasIndex(x => x.HoldNumber).IsUnique();

            builder.HasOne(x => x.Warehouse)
                .WithMany()
                .HasForeignKey(x => x.WarehouseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.PaymentMethod)
                .WithMany()
                .HasForeignKey(x => x.PaymentMethodId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
