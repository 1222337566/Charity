using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Rfp
{
    public class HrEmployeeContractConfiguration : IEntityTypeConfiguration<HrEmployeeContract>
    {
        public void Configure(EntityTypeBuilder<HrEmployeeContract> builder)
        {
            builder.ToTable("CharityHrEmployeeContracts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ContractType).IsRequired().HasMaxLength(100);
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
