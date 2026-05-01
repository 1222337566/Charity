using InfrastrfuctureManagmentCore.Domains.HR.Advanced;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.HR.Advanced
{
    public class HrEmployeeMovementConfiguration : IEntityTypeConfiguration<HrEmployeeMovement>
    {
        public void Configure(EntityTypeBuilder<HrEmployeeMovement> builder)
        {
            builder.ToTable("HrEmployeeMovements");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.MovementType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.DecisionNumber).HasMaxLength(100);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.HasIndex(x => new { x.EmployeeId, x.EffectiveDate });
        }
    }
}
