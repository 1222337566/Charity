using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectActivityConfiguration : IEntityTypeConfiguration<ProjectActivity>
    {
        public void Configure(EntityTypeBuilder<ProjectActivity> builder)
        {
            builder.ToTable("CharityProjectActivities");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Title).HasMaxLength(250).IsRequired();
            builder.Property(x => x.Description).HasMaxLength(4000);
            builder.Property(x => x.Status).HasMaxLength(100).IsRequired();
            builder.Property(x => x.PlannedCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ActualCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ProjectId, x.Status });
        }
    }
}
