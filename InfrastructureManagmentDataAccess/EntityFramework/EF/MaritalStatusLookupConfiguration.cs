using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class MaritalStatusLookupConfiguration : IEntityTypeConfiguration<MaritalStatusLookup>
    {
        public void Configure(EntityTypeBuilder<MaritalStatusLookup> builder)
        {
            builder.ToTable("CharityMaritalStatuses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameEn).HasMaxLength(100);

            builder.HasData(
                new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalSingle, NameAr = "أعزب", NameEn = "Single", DisplayOrder = 1, IsActive = true },
                new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalMarried, NameAr = "متزوج", NameEn = "Married", DisplayOrder = 2, IsActive = true },
                new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalWidowed, NameAr = "أرمل", NameEn = "Widowed", DisplayOrder = 3, IsActive = true },
                new MaritalStatusLookup { Id = CharityLookupSeedIds.MaritalDivorced, NameAr = "مطلق", NameEn = "Divorced", DisplayOrder = 4, IsActive = true });
        }
    }
}
