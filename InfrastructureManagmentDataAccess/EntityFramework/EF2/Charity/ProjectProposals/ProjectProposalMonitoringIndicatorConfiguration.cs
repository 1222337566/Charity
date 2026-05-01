using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalMonitoringIndicatorConfiguration : IEntityTypeConfiguration<ProjectProposalMonitoringIndicator>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalMonitoringIndicator> builder)
        {

            builder.ToTable("CharityProjectProposalMonitoringIndicators");
            builder.HasKey(x => x.Id);
            builder.HasIndex(x => x.ProjectProposalId);
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.MonitoringIndicators)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
