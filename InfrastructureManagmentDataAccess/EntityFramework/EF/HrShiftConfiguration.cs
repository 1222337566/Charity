using InfrastrfuctureManagmentCore.Domains.HR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class HrShiftConfiguration : IEntityTypeConfiguration<HrShift>
    {
        public void Configure(EntityTypeBuilder<HrShift> builder)
        {
            builder.ToTable("CharityHrShifts");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.HasIndex(x => x.Name).IsUnique();
        }
    }
}
