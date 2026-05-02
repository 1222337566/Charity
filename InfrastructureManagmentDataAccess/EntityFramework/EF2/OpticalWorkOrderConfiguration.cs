using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    using InfrastrfuctureManagmentCore.Domains.Optics;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OpticalWorkOrderConfiguration : IEntityTypeConfiguration<OpticalWorkOrder>
    {
        public void Configure(EntityTypeBuilder<OpticalWorkOrder> builder)
        {
            builder.ToTable("OpticalWorkOrders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.WorkOrderNumber)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(x => x.FrameNotes)
                .HasMaxLength(1000);

            builder.Property(x => x.LensNotes)
                .HasMaxLength(1000);

            builder.Property(x => x.WorkshopNotes)
                .HasMaxLength(2000);

            builder.Property(x => x.DeliveryNotes)
                .HasMaxLength(1000);

            builder.HasIndex(x => x.WorkOrderNumber).IsUnique();

            builder.HasIndex(x => x.SalesInvoiceId).IsUnique();

            builder.HasOne(x => x.SalesInvoice)
                .WithMany()
                .HasForeignKey(x => x.SalesInvoiceId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Prescription)
                .WithMany()
                .HasForeignKey(x => x.PrescriptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
