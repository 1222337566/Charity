using InfrastrfuctureManagmentCore.Domains.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class PayrollMonthConfiguration : IEntityTypeConfiguration<PayrollMonth>
    {
        public void Configure(EntityTypeBuilder<PayrollMonth> builder)
        {
            builder.ToTable("CharityPayrollMonths");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.HasIndex(x => new { x.Year, x.Month }).IsUnique();
        }
    }
}
