using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectPhaseExpenseLinkConfiguration : IEntityTypeConfiguration<ProjectPhaseExpenseLink>
    {
        public void Configure(EntityTypeBuilder<ProjectPhaseExpenseLink> builder)
        {
            builder.ToTable("ProjectPhaseExpenseLinks");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes).HasMaxLength(2000);
            builder.HasIndex(x => x.ExpenseId).IsUnique();
            builder.HasIndex(x => new { x.ProjectId, x.ProjectPhaseId });

            builder.HasOne(x => x.Project)
                .WithMany()
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProjectPhase)
                .WithMany()
                .HasForeignKey(x => x.ProjectPhaseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.ProjectBudgetLine)
                .WithMany()
                .HasForeignKey(x => x.ProjectBudgetLineId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CostCenter)
                .WithMany()
                .HasForeignKey(x => x.CostCenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
