using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalTargetGroupConfiguration : IEntityTypeConfiguration<ProjectProposalTargetGroup>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalTargetGroup> builder)
        {

            builder.ToTable("CharityProjectProposalTargetGroups");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.TargetGroups)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
