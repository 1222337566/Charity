using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalConfiguration : IEntityTypeConfiguration<ProjectProposal>
    {
        public void Configure(EntityTypeBuilder<ProjectProposal> builder)
        {

            builder.ToTable("CharityProjectProposals");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.ProposalNumber).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Title).HasMaxLength(500).IsRequired();
            builder.Property(x => x.DonorName).HasMaxLength(250);
            builder.Property(x => x.OrganizationName).HasMaxLength(250);
            builder.Property(x => x.ProjectLocation).HasMaxLength(500);
            builder.Property(x => x.Currency).HasMaxLength(20);
            builder.Property(x => x.Status).HasMaxLength(50);
            builder.HasIndex(x => x.ProposalNumber).IsUnique();

        }
    }
}
