using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Rfp
{
    public class HrEmployeeBonusConfiguration : IEntityTypeConfiguration<HrEmployeeBonus>
    {
        public void Configure(EntityTypeBuilder<HrEmployeeBonus> builder)
        {
            builder.ToTable("CharityHrEmployeeBonuses");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BonusType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
