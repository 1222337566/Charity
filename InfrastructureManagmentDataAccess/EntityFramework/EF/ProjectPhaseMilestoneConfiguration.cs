using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectPhaseMilestoneConfiguration : IEntityTypeConfiguration<ProjectPhaseMilestone>
    {
        public void Configure(EntityTypeBuilder<ProjectPhaseMilestone> builder)
        {
            builder.ToTable("CharityProjectPhaseMilestones");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            builder.HasIndex(x => new { x.ProjectPhaseId, x.DueDate });
            builder.HasOne(x => x.Phase)
                .WithMany(x => x.Milestones)
                .HasForeignKey(x => x.ProjectPhaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
