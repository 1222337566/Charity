using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalTeamMemberConfiguration : IEntityTypeConfiguration<ProjectProposalTeamMember>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalTeamMember> builder)
        {

            builder.ToTable("CharityProjectProposalTeamMembers");
            builder.HasKey(x => x.Id);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.TeamMembers)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
