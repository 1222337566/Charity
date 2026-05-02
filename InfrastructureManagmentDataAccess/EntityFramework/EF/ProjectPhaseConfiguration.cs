using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectPhaseConfiguration : IEntityTypeConfiguration<ProjectPhase>
    {
        public void Configure(EntityTypeBuilder<ProjectPhase> builder)
        {
            builder.ToTable("CharityProjectPhases");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Code).HasMaxLength(50);
            builder.Property(x => x.Name).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            builder.Property(x => x.PlannedCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ActualCost).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ResponsiblePersonName).HasMaxLength(150);
            builder.HasIndex(x => new { x.ProjectId, x.SortOrder });
            builder.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
