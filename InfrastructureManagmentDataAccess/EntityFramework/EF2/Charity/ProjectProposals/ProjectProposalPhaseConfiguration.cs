using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalPhaseConfiguration : IEntityTypeConfiguration<ProjectProposalPhase>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalPhase> b)
        {
            b.ToTable("CharityProjectProposalPhases");
            b.HasKey(x => x.Id);
            b.Property(x => x.Name).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).HasMaxLength(500);
            b.HasOne(x => x.Proposal)
                .WithMany(x => x.Phases)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
