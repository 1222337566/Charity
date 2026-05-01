using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Configurations.Charity;

public class BeneficiaryDocumentConfiguration : IEntityTypeConfiguration<BeneficiaryDocument>
{
    public void Configure(EntityTypeBuilder<BeneficiaryDocument> builder)
    {
        builder.ToTable("CharityBeneficiaryDocuments");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.DocumentType).HasMaxLength(100).IsRequired();
        builder.Property(x => x.OriginalFileName).HasMaxLength(260).IsRequired();
        builder.Property(x => x.ContentType).HasMaxLength(150).IsRequired();
        builder.Property(x => x.FileExtension).HasMaxLength(20).IsRequired();
        builder.Property(x => x.FileContent).HasColumnType("varbinary(max)").IsRequired();

      
    }
}
