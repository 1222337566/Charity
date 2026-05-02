using InfrastrfuctureManagmentCore.Domains.OrganizationDocuments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class OrganizationDocumentConfiguration : IEntityTypeConfiguration<OrganizationDocument>
    {
        public void Configure(EntityTypeBuilder<OrganizationDocument> builder)
        {
            builder.ToTable("OrganizationDocuments");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DocumentNumber)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(x => x.Title)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(x => x.DocumentType)
                .HasMaxLength(80)
                .IsRequired();

            builder.Property(x => x.RelatedEntityType)
                .HasMaxLength(100);

            builder.Property(x => x.RelatedEntityName)
                .HasMaxLength(300);

            builder.Property(x => x.FileName)
                .HasMaxLength(260)
                .IsRequired();

            builder.Property(x => x.ContentType)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.FileContent)
                .IsRequired();

            builder.Property(x => x.Notes)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedByUserId)
                .HasMaxLength(450);

            builder.HasIndex(x => x.DocumentNumber);
            builder.HasIndex(x => x.DocumentType);
            builder.HasIndex(x => x.DocumentDateUtc);
            builder.HasIndex(x => new { x.RelatedEntityType, x.RelatedEntityId });
            builder.HasIndex(x => x.IsArchived);
        }
    }
}
