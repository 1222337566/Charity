using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalPhaseActivityConfiguration : IEntityTypeConfiguration<ProjectProposalPhaseActivity>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalPhaseActivity> b)
        {
            b.ToTable("CharityProjectProposalPhaseActivities");
            b.HasKey(x => x.Id);
            b.Property(x => x.ActivityTitle).IsRequired().HasMaxLength(300);
            b.HasOne(x => x.Phase)
                .WithMany(x => x.Activities)
                .HasForeignKey(x => x.PhaseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
