using InfrastrfuctureManagmentCore.Domains.Payroll;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class SalaryItemDefinitionConfiguration : IEntityTypeConfiguration<SalaryItemDefinition>
    {
        public void Configure(EntityTypeBuilder<SalaryItemDefinition> builder)
        {
            builder.ToTable("CharitySalaryItemDefinitions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.ItemType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.CalculationMethod).HasMaxLength(50).IsRequired();
            builder.Property(x => x.DefaultValue).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
