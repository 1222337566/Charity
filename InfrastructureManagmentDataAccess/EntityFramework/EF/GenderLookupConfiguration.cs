using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class GenderLookupConfiguration : IEntityTypeConfiguration<GenderLookup>
    {
        public void Configure(EntityTypeBuilder<GenderLookup> builder)
        {
            builder.ToTable("CharityGenders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameEn).HasMaxLength(100);

            builder.HasData(
                new GenderLookup
                {
                    Id = CharityLookupSeedIds.GenderMale,
                    NameAr = "ذكر",
                    NameEn = "Male",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new GenderLookup
                {
                    Id = CharityLookupSeedIds.GenderFemale,
                    NameAr = "أنثى",
                    NameEn = "Female",
                    DisplayOrder = 2,
                    IsActive = true
                });
        }
    }
}
