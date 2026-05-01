using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalPastExperienceConfiguration : IEntityTypeConfiguration<ProjectProposalPastExperience>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalPastExperience> builder)
        {

            builder.ToTable("CharityProjectProposalPastExperiences");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectProposalId);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.PastExperiences)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
