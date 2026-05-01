using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardMeetingAttachmentConfiguration : IEntityTypeConfiguration<BoardMeetingAttachment>
    {
        public void Configure(EntityTypeBuilder<BoardMeetingAttachment> builder)
        {
            builder.ToTable("CharityBoardMeetingAttachments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.FileName).HasMaxLength(260).IsRequired();
            builder.Property(x => x.FilePath).HasMaxLength(1000);
            builder.Property(x => x.AttachmentType).HasMaxLength(100);
        }
    }
}
