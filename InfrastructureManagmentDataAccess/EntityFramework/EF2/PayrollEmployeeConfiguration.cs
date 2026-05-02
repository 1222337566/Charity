using InfrastrfuctureManagmentCore.Domains.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class PayrollEmployeeConfiguration : IEntityTypeConfiguration<PayrollEmployee>
    {
        public void Configure(EntityTypeBuilder<PayrollEmployee> builder)
        {
            builder.ToTable("CharityPayrollEmployees");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BasicSalary).HasColumnType("decimal(18,2)");
            builder.Property(x => x.AttendanceDeduction).HasColumnType("decimal(18,2)");
            builder.Property(x => x.OtherDeductions).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Additions).HasColumnType("decimal(18,2)");
            builder.Property(x => x.GrossAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalDeductions).HasColumnType("decimal(18,2)");
            builder.Property(x => x.NetAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.HasIndex(x => new { x.PayrollMonthId, x.EmployeeId }).IsUnique();

            builder.HasOne(x => x.PayrollMonth)
                .WithMany(x => x.Employees)
                .HasForeignKey(x => x.PayrollMonthId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
