using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectTaskDailyUpdateConfiguration : IEntityTypeConfiguration<ProjectTaskDailyUpdate>
    {
        public void Configure(EntityTypeBuilder<ProjectTaskDailyUpdate> builder)
        {
            builder.ToTable("CharityProjectTaskDailyUpdates");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Status).HasMaxLength(50).IsRequired();
            builder.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            builder.Property(x => x.HoursSpent).HasColumnType("decimal(18,2)");
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);
            builder.Property(x => x.CreatedByName).HasMaxLength(150);
            builder.HasIndex(x => new { x.TaskId, x.UpdateDate });
            builder.HasOne(x => x.Task)
                .WithMany(x => x.DailyUpdates)
                .HasForeignKey(x => x.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
