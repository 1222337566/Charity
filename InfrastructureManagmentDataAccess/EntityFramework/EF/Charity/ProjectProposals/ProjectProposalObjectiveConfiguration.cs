using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalObjectiveConfiguration : IEntityTypeConfiguration<ProjectProposalObjective>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalObjective> builder)
        {

            builder.ToTable("CharityProjectProposalObjectives");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.Objectives)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
