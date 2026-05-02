using InfrastrfuctureManagmentCore.Domains.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class EmployeeSalaryStructureConfiguration : IEntityTypeConfiguration<EmployeeSalaryStructure>
    {
        public void Configure(EntityTypeBuilder<EmployeeSalaryStructure> builder)
        {
            builder.ToTable("CharityEmployeeSalaryStructures");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Value).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.Employee)
                .WithMany()
                .HasForeignKey(x => x.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.SalaryItemDefinition)
                .WithMany(x => x.EmployeeSalaryStructures)
                .HasForeignKey(x => x.SalaryItemDefinitionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
