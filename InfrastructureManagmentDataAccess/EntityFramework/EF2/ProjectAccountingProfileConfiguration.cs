using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectAccountingProfileConfiguration : IEntityTypeConfiguration<ProjectAccountingProfile>
    {
        public void Configure(EntityTypeBuilder<ProjectAccountingProfile> builder)
        {
            builder.ToTable("ProjectAccountingProfiles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.HasIndex(x => x.ProjectId).IsUnique();

            builder.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.DefaultCostCenter).WithMany().HasForeignKey(x => x.DefaultCostCenterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.DefaultRevenueAccount).WithMany().HasForeignKey(x => x.DefaultRevenueAccountId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.DefaultExpenseAccount).WithMany().HasForeignKey(x => x.DefaultExpenseAccountId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
