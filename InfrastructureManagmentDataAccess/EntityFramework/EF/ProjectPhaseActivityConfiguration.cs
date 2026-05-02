using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectPhaseActivityConfiguration : IEntityTypeConfiguration<ProjectPhaseActivity>
    {
        public void Configure(EntityTypeBuilder<ProjectPhaseActivity> builder)
        {
            builder.ToTable("CharityProjectPhaseActivities");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(50);
            builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Priority).HasMaxLength(20).IsRequired();
            builder.Property(x => x.ResponsiblePersonName).HasMaxLength(150);
            builder.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            builder.Property(x => x.PlannedHours).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ActualHours).HasColumnType("decimal(18,2)");
            builder.HasIndex(x => new { x.PhaseId, x.SortOrder });
            builder.HasOne(x => x.Phase)
                .WithMany()
                .HasForeignKey(x => x.PhaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
