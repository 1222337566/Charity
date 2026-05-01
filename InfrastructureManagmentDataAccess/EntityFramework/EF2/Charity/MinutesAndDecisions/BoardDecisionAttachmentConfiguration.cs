using InfrastrfuctureManagmentCore.Domains.Charity.MinutesAndDecisions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.MinutesAndDecisions
{
    public class BoardDecisionAttachmentConfiguration : IEntityTypeConfiguration<BoardDecisionAttachment>
    {
        public void Configure(EntityTypeBuilder<BoardDecisionAttachment> builder)
        {
            builder.ToTable("CharityBoardDecisionAttachments");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
            builder.Property(x => x.StoredFileName).HasMaxLength(260).IsRequired();
            builder.Property(x => x.ContentType).HasMaxLength(200).IsRequired();
            builder.Property(x => x.FileExtension).HasMaxLength(20).IsRequired();
            builder.Property(x => x.FileSizeBytes).IsRequired();
            builder.Property(x => x.FileContent).HasColumnType("varbinary(max)").IsRequired();
            builder.Property(x => x.AttachmentType).HasMaxLength(100);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);

            builder.HasOne(x => x.BoardDecision)
                .WithMany(x => x.Attachments)
                .HasForeignKey(x => x.BoardDecisionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
