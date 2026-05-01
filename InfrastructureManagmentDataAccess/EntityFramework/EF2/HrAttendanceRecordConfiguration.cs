using InfrastrfuctureManagmentCore.Domains.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class HrAttendanceRecordConfiguration : IEntityTypeConfiguration<HrAttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<HrAttendanceRecord> builder)
        {
            builder.ToTable("CharityHrAttendanceRecords");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Source).HasMaxLength(50).IsRequired();
            builder.Property(x => x.WorkedHours).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasIndex(x => new { x.EmployeeId, x.AttendanceDate }).IsUnique();

            builder.HasOne(x => x.Employee)
                .WithMany(x => x.AttendanceRecords)
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Shift)
                .WithMany(x => x.AttendanceRecords)
                .HasForeignKey(x => x.ShiftId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
