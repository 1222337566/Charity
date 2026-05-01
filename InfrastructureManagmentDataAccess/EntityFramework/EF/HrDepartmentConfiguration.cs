using InfrastrfuctureManagmentCore.Domains.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class HrDepartmentConfiguration : IEntityTypeConfiguration<HrDepartment>
    {
        public void Configure(EntityTypeBuilder<HrDepartment> builder)
        {
            builder.ToTable("CharityHrDepartments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(1000);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
