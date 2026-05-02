using InfrastrfuctureManagmentCore.Domains.Charity.ProjectProposals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.ProjectProposals
{
    public class ProjectProposalAttachmentConfiguration : IEntityTypeConfiguration<ProjectProposalAttachment>
    {
        public void Configure(EntityTypeBuilder<ProjectProposalAttachment> builder)
        {

            builder.ToTable("CharityProjectProposalAttachments");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
            builder.Property(x => x.FileExtension).HasMaxLength(20).IsRequired();
            builder.Property(x => x.FileContent).HasColumnType("varbinary(max)").IsRequired();
            builder.HasOne(x => x.Proposal)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.ProjectProposalId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
