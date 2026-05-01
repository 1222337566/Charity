using InfrastrfuctureManagmentCore.Domains.Charity.AidCycles;
using InfrastructureManagmentCore.Domains.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AidCycleConfiguration : IEntityTypeConfiguration<AidCycle>
    {
        public void Configure(EntityTypeBuilder<AidCycle> builder)
        {
            builder.ToTable("CharityAidCycles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.CycleNumber).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Title).IsRequired().HasMaxLength(200);
            builder.Property(x => x.CycleType).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Status).IsRequired().HasMaxLength(30);
            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.Property(x => x.TotalPlannedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.TotalDisbursedAmount).HasColumnType("decimal(18,2)");

            builder.HasIndex(x => x.CycleNumber).IsUnique();
            builder.HasIndex(x => new { x.PeriodYear, x.PeriodMonth, x.CycleType });

            builder.HasOne(x => x.AidType)
                .WithMany()
                .HasForeignKey(x => x.AidTypeId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.CreatedByUserId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne<ApplicationUser>().WithMany()
                .HasForeignKey(x => x.ApprovedByUserId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
