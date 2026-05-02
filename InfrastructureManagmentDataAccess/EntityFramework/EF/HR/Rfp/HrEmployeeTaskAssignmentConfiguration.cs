using InfrastrfuctureManagmentCore.Domains.HR.Rfp;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Rfp
{
    public class HrEmployeeTaskAssignmentConfiguration : IEntityTypeConfiguration<HrEmployeeTaskAssignment>
    {
        public void Configure(EntityTypeBuilder<HrEmployeeTaskAssignment> builder)
        {
            builder.ToTable("CharityHrEmployeeTaskAssignments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.TaskTitle).IsRequired().HasMaxLength(250);
            builder.Property(x => x.CreatedAtUtc).IsRequired();
            builder.HasOne(x => x.Employee).WithMany().HasForeignKey(x => x.EmployeeId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
