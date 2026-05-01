using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Rfp
{
    public class HrEmployeeFundingAssignmentConfiguration : IEntityTypeConfiguration<HrEmployeeFundingAssignment>
    {
        public void Configure(EntityTypeBuilder<HrEmployeeFundingAssignment> builder)
        {
            builder.ToTable("CharityHrEmployeeFundingAssignments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.FundingSourceName).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
