using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectTrackingLogConfiguration : IEntityTypeConfiguration<ProjectTrackingLog>
    {
        public void Configure(EntityTypeBuilder<ProjectTrackingLog> builder)
        {
            builder.ToTable("CharityProjectTrackingLogs");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.EntryType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50);
            builder.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            builder.HasIndex(x => new { x.ProjectId, x.EntryDate });
            builder.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Phase)
                .WithMany(x => x.TrackingLogs)
                .HasForeignKey(x => x.ProjectPhaseId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
