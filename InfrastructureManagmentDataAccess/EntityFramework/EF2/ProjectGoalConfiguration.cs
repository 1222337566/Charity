using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity
{
    public class ProjectGoalConfiguration : IEntityTypeConfiguration<ProjectGoal>
    {
        public void Configure(EntityTypeBuilder<ProjectGoal> b)
        {
            b.ToTable("CharityProjectGoals");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(300);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.SuccessIndicator).HasMaxLength(500);
            b.Property(x => x.TargetValue).HasMaxLength(200);
            b.Property(x => x.AchievedValue).HasMaxLength(200);
            b.Property(x => x.Status).HasMaxLength(20);
            b.HasIndex(x => x.ProjectId);
            b.HasOne(x => x.Project).WithMany(p => p.Goals)
                .HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ProjectSubGoalConfiguration : IEntityTypeConfiguration<ProjectSubGoal>
    {
        public void Configure(EntityTypeBuilder<ProjectSubGoal> b)
        {
            b.ToTable("CharityProjectSubGoals");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(300);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.SuccessIndicator).HasMaxLength(500);
            b.Property(x => x.TargetValue).HasMaxLength(200);
            b.Property(x => x.AchievedValue).HasMaxLength(200);
            b.Property(x => x.Status).HasMaxLength(20);
            b.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            b.HasIndex(x => x.GoalId);
            b.HasOne(x => x.Goal).WithMany(g => g.SubGoals)
                .HasForeignKey(x => x.GoalId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ProjectSubGoalActivityConfiguration : IEntityTypeConfiguration<ProjectSubGoalActivity>
    {
        public void Configure(EntityTypeBuilder<ProjectSubGoalActivity> b)
        {
            b.ToTable("CharityProjectSubGoalActivities");
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(300);
            b.Property(x => x.Description).HasMaxLength(2000);
            b.Property(x => x.TargetGroup).HasMaxLength(200);
            b.Property(x => x.TargetGroupDescription).HasMaxLength(500);
            b.Property(x => x.QuantityUnit).HasMaxLength(50);
            b.Property(x => x.Status).HasMaxLength(20);
            b.Property(x => x.Priority).HasMaxLength(20);
            b.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            b.Property(x => x.PlannedQuantity).HasColumnType("decimal(18,2)");
            b.Property(x => x.ActualQuantity).HasColumnType("decimal(18,2)");
            b.Property(x => x.PlannedCost).HasColumnType("decimal(18,2)");
            b.Property(x => x.ActualCost).HasColumnType("decimal(18,2)");
            b.Property(x => x.PlannedHoursPerDay).HasColumnType("decimal(5,2)");
            b.Property(x => x.PerformanceIndicator).HasMaxLength(500);
            b.Property(x => x.VerificationMeans).HasMaxLength(500);
            b.Property(x => x.TargetAchievement).HasMaxLength(50);
            b.HasIndex(x => x.SubGoalId);
            // ← لا يوجد PhaseId FK هنا — النشاط يرتبط بالمراحل عبر ActivityPhaseAssignment
            b.HasOne(x => x.SubGoal).WithMany(s => s.Activities)
                .HasForeignKey(x => x.SubGoalId).OnDelete(DeleteBehavior.Cascade);
        }
    }

    public class ActivityPhaseAssignmentConfiguration : IEntityTypeConfiguration<ActivityPhaseAssignment>
    {
        public void Configure(EntityTypeBuilder<ActivityPhaseAssignment> b)
        {
            b.ToTable("CharityActivityPhaseAssignments");
            b.HasKey(x => x.Id);
            b.Property(x => x.ContributionPercent).HasColumnType("decimal(5,2)");
            b.Property(x => x.ProgressPercent).HasColumnType("decimal(5,2)");
            b.Property(x => x.PlannedQuantity).HasColumnType("decimal(18,2)");
            b.Property(x => x.ActualQuantity).HasColumnType("decimal(18,2)");
            b.Property(x => x.PlannedCost).HasColumnType("decimal(18,2)");
            b.Property(x => x.ActualCost).HasColumnType("decimal(18,2)");
            b.Property(x => x.PlannedHoursPerDay).HasColumnType("decimal(5,2)");
            b.Property(x => x.Status).HasMaxLength(20);
            b.Property(x => x.Notes).HasMaxLength(1000);
            b.HasIndex(x => x.ActivityId);
            b.HasIndex(x => x.PhaseId);
            b.HasOne(x => x.Activity).WithMany(a => a.PhaseAssignments)
                .HasForeignKey(x => x.ActivityId).OnDelete(DeleteBehavior.Cascade);
            b.HasOne(x => x.Phase).WithMany()
                .HasForeignKey(x => x.PhaseId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
