using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InfrastrfuctureManagmentCore.Domains.Prescriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
   

    public class PrescriptionConfiguration : IEntityTypeConfiguration<Prescription>
    {
        public void Configure(EntityTypeBuilder<Prescription> builder)
        {
            builder.ToTable("Prescriptions");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DoctorName)
                .HasMaxLength(200);

            builder.Property(x => x.RightSph).HasColumnType("decimal(8,2)");
            builder.Property(x => x.RightCyl).HasColumnType("decimal(8,2)");
            builder.Property(x => x.RightAxis).HasColumnType("decimal(8,2)");

            builder.Property(x => x.LeftSph).HasColumnType("decimal(8,2)");
            builder.Property(x => x.LeftCyl).HasColumnType("decimal(8,2)");
            builder.Property(x => x.LeftAxis).HasColumnType("decimal(8,2)");

            builder.Property(x => x.AddValue).HasColumnType("decimal(8,2)");
            builder.Property(x => x.IPD).HasColumnType("decimal(8,2)");
            builder.Property(x => x.SHeight).HasColumnType("decimal(8,2)");

            builder.Property(x => x.Remarks)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Customer)
                .WithMany()
                .HasForeignKey(x => x.CustomerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
