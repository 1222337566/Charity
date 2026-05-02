using InfrastrfuctureManagmentCore.Domains.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class PayrollEmployeeItemConfiguration : IEntityTypeConfiguration<PayrollEmployeeItem>
    {
        public void Configure(EntityTypeBuilder<PayrollEmployeeItem> builder)
        {
            builder.ToTable("CharityPayrollEmployeeItems");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ItemType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Value).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.PayrollEmployee)
                .WithMany(x => x.Items)
                .HasForeignKey(x => x.PayrollEmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SalaryItemDefinition)
                .WithMany(x => x.PayrollEmployeeItems)
                .HasForeignKey(x => x.SalaryItemDefinitionId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
