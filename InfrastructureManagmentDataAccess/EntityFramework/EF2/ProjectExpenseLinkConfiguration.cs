using InfrastrfuctureManagmentCore.Domains.Accounting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class ProjectExpenseLinkConfiguration : IEntityTypeConfiguration<ProjectExpenseLink>
    {
        public void Configure(EntityTypeBuilder<ProjectExpenseLink> builder)
        {
            builder.ToTable("ProjectExpenseLinks");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.HasIndex(x => x.ExpenseId).IsUnique();

            builder.HasOne(x => x.Expense).WithMany().HasForeignKey(x => x.ExpenseId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Project).WithMany().HasForeignKey(x => x.ProjectId).OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.CostCenter).WithMany().HasForeignKey(x => x.CostCenterId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.ProjectBudgetLine).WithMany().HasForeignKey(x => x.ProjectBudgetLineId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
