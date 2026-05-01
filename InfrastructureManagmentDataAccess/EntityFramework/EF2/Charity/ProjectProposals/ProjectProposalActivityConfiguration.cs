using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalActivityConfiguration : IEntityTypeConfiguration<ProjectProposalActivity>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalActivity> builder)
        {

            builder.ToTable("CharityProjectProposalActivitys");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectProposalId);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
