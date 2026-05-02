using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectPhaseTaskConfiguration : IEntityTypeConfiguration<ProjectPhaseTask>
    {
        public void Configure(EntityTypeBuilder<ProjectPhaseTask> builder)
        {
            builder.ToTable("CharityProjectPhaseTasks");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(50);
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Priority).HasMaxLength(20).IsRequired();
            builder.Property(x => x.AssignedToUserId).HasMaxLength(450);
            builder.Property(x => x.AssignedToName).HasMaxLength(150);
            builder.Property(x => x.PercentComplete).HasColumnType("decimal(5,2)");
            builder.Property(x => x.EstimatedHours).HasColumnType("decimal(18,2)");
            builder.Property(x => x.SpentHours).HasColumnType("decimal(18,2)");
            builder.HasIndex(x => new { x.ActivityId, x.SortOrder });
            builder.HasIndex(x => new { x.ProjectId, x.Status });
            builder.HasOne(x => x.Activity)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ActivityId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Phase)
                .WithMany()
                .HasForeignKey(x => x.PhaseId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
