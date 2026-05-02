using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Advanced
{
    public class HrSanctionRecordConfiguration : IEntityTypeConfiguration<HrSanctionRecord>
    {
        public void Configure(EntityTypeBuilder<HrSanctionRecord> builder)
        {
            builder.ToTable("HrSanctionRecords");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SanctionType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Reason).HasMaxLength(1000).IsRequired();
            builder.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.HasIndex(x => new { x.EmployeeId, x.SanctionDate });
        }
    }
}
