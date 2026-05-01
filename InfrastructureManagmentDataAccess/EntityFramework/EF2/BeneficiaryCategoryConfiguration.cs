using InfrastrfuctureManagmentCore.Domains.Charity.Beneficiaries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Beneficiaries
{
    public class BeneficiaryCategoryConfiguration : IEntityTypeConfiguration<BeneficiaryCategory>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryCategory> builder)
        {
            builder.ToTable("CharityBeneficiaryCategories");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.Code)
                .HasMaxLength(100);

            builder.Property(x => x.Description)
                .HasMaxLength(1000);

            builder.Property(x => x.CreatedByUserId)
                .HasMaxLength(450);

            builder.HasIndex(x => x.Code)
                .IsUnique()
                .HasFilter("[Code] IS NOT NULL");

            builder.HasIndex(x => x.NameAr);
            builder.HasIndex(x => x.IsWaitingListCategory);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
