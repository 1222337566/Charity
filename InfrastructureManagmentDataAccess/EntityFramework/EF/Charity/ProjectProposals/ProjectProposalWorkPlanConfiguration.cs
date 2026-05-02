using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalWorkPlanConfiguration : IEntityTypeConfiguration<ProjectProposalWorkPlan>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalWorkPlan> builder)
        {

            builder.ToTable("CharityProjectProposalWorkPlans");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.WorkPlanItems)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
