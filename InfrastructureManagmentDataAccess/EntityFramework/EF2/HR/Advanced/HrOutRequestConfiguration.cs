using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Advanced
{
    public class HrOutRequestConfiguration : IEntityTypeConfiguration<HrOutRequest>
    {
        public void Configure(EntityTypeBuilder<HrOutRequest> builder)
        {
            builder.ToTable("HrOutRequests");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(30).IsRequired();
            builder.Property(x => x.ApprovedByUserId).HasMaxLength(450);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.HasIndex(x => new { x.EmployeeId, x.OutDate });
            builder.HasIndex(x => x.Status);
        }
    }
}
