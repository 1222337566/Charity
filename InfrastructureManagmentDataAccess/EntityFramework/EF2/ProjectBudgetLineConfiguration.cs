using InfrastrfuctureManagmentCore.Domains.Charity.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectBudgetLineConfiguration : IEntityTypeConfiguration<ProjectBudgetLine>
    {
        public void Configure(EntityTypeBuilder<ProjectBudgetLine> builder)
        {
            builder.ToTable("CharityProjectBudgetLines");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.LineName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.LineType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.PlannedAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.ActualAmount).HasColumnType("decimal(18,2)");
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasOne(x => x.Project)
                .WithMany(x => x.BudgetLines)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => new { x.ProjectId, x.LineType });
        }
    }
}
