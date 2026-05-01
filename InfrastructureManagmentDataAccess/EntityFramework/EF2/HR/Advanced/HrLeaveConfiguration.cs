using InfrastrfuctureManagmentCore.Domains.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Advanced
{
    public class HrLeaveTypeConfiguration : IEntityTypeConfiguration<HrLeaveType>
    {
        public void Configure(EntityTypeBuilder<HrLeaveType> b)
        {
            b.ToTable("HrLeaveTypes");
            b.HasKey(x => x.Id);
            b.Property(x => x.Code).IsRequired().HasMaxLength(20);
            b.Property(x => x.NameAr).IsRequired().HasMaxLength(100);
            b.Property(x => x.NameEn).HasMaxLength(100);
            b.Property(x => x.Category).HasMaxLength(30);
            b.Property(x => x.Color).HasMaxLength(20);
            b.HasIndex(x => x.Code).IsUnique();
            b.HasData(
                new HrLeaveType { Id = Guid.Parse("a1000001-0000-0000-0000-000000000001"), Code = "ANNUAL", NameAr = "إجازة سنوية",      NameEn = "Annual Leave",    Category = "Annual",    MaxDaysPerYear = 21, MaxConsecutiveDays = 21, PaidLeave = true,  CarryOverAllowed = true,  MaxCarryOverDays = 7,  Color = "#0e6f73", IsActive = true },
                new HrLeaveType { Id = Guid.Parse("a1000002-0000-0000-0000-000000000002"), Code = "SICK",   NameAr = "إجازة مرضية",      NameEn = "Sick Leave",      Category = "Sick",      MaxDaysPerYear = 90, MaxConsecutiveDays = 30, PaidLeave = true,  CarryOverAllowed = false, MaxCarryOverDays = 0,  Color = "#f46a6a", RequiresAttachment = true, IsActive = true },
                new HrLeaveType { Id = Guid.Parse("a1000003-0000-0000-0000-000000000003"), Code = "EMRG",  NameAr = "إجازة طارئة",     NameEn = "Emergency Leave", Category = "Emergency", MaxDaysPerYear = 6,  MaxConsecutiveDays = 3,  PaidLeave = true,  CarryOverAllowed = false, MaxCarryOverDays = 0,  Color = "#f1b44c", IsActive = true },
                new HrLeaveType { Id = Guid.Parse("a1000004-0000-0000-0000-000000000004"), Code = "UNPAID",NameAr = "إجازة بدون راتب",  NameEn = "Unpaid Leave",    Category = "Unpaid",    MaxDaysPerYear = 60, MaxConsecutiveDays = 30, PaidLeave = false, CarryOverAllowed = false, MaxCarryOverDays = 0,  Color = "#adb5bd", IsActive = true },
                new HrLeaveType { Id = Guid.Parse("a1000005-0000-0000-0000-000000000005"), Code = "MAT",   NameAr = "إجازة أمومة",     NameEn = "Maternity Leave", Category = "Maternity", MaxDaysPerYear = 90, MaxConsecutiveDays = 90, PaidLeave = true,  CarryOverAllowed = false, MaxCarryOverDays = 0,  Color = "#f472b6", IsActive = true },
                new HrLeaveType { Id = Guid.Parse("a1000006-0000-0000-0000-000000000006"), Code = "PAT",   NameAr = "إجازة أبوة",      NameEn = "Paternity Leave", Category = "Paternity", MaxDaysPerYear = 3,  MaxConsecutiveDays = 3,  PaidLeave = true,  CarryOverAllowed = false, MaxCarryOverDays = 0,  Color = "#818cf8", IsActive = true },
                new HrLeaveType { Id = Guid.Parse("a1000007-0000-0000-0000-000000000007"), Code = "HAJJ",  NameAr = "إجازة الحج",      NameEn = "Hajj Leave",      Category = "Hajj",      MaxDaysPerYear = 30, MaxConsecutiveDays = 30, PaidLeave = true,  CarryOverAllowed = false, MaxCarryOverDays = 0,  Color = "#34c38f", IsActive = true }
            );
        }
    }

    public class HrLeaveRequestConfiguration : IEntityTypeConfiguration<HrLeaveRequest>
    {
        public void Configure(EntityTypeBuilder<HrLeaveRequest> b)
        {
            b.ToTable("HrLeaveRequests");
            b.HasKey(x => x.Id);
            b.Property(x => x.RequestNumber).IsRequired().HasMaxLength(30);
            b.Property(x => x.Status).HasMaxLength(20);
            b.Property(x => x.Reason).HasMaxLength(1000);
            b.Property(x => x.RejectionReason).HasMaxLength(500);
            b.Property(x => x.AttachmentPath).HasMaxLength(500);
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.Property(x => x.HalfDayPeriod).HasMaxLength(20);
            b.Property(x => x.CreatedByUserId).HasMaxLength(450);
            b.Property(x => x.ApprovedByUserId).HasMaxLength(450);
            b.HasIndex(x => x.EmployeeId);
            b.HasIndex(x => new { x.EmployeeId, x.StartDate });
            b.HasIndex(x => x.Status);
            b.HasOne(x => x.Employee).WithMany()
                .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.LeaveType).WithMany()
                .HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }

    public class HrLeaveBalanceConfiguration : IEntityTypeConfiguration<HrLeaveBalance>
    {
        public void Configure(EntityTypeBuilder<HrLeaveBalance> b)
        {
            b.ToTable("HrLeaveBalances");
            b.HasKey(x => x.Id);
            b.Property(x => x.TotalEntitled).HasColumnType("decimal(8,2)");
            b.Property(x => x.TotalUsed).HasColumnType("decimal(8,2)");
            b.Property(x => x.TotalPending).HasColumnType("decimal(8,2)");
            b.Property(x => x.CarriedOver).HasColumnType("decimal(8,2)");
            b.Ignore(x => x.Remaining); // computed property
            b.HasIndex(x => new { x.EmployeeId, x.LeaveTypeId, x.Year }).IsUnique();
            b.HasOne(x => x.Employee).WithMany()
                .HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.LeaveType).WithMany()
                .HasForeignKey(x => x.LeaveTypeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
