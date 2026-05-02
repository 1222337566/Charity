using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AidTypeLookupConfiguration : IEntityTypeConfiguration<AidTypeLookup>
    {
        public void Configure(EntityTypeBuilder<AidTypeLookup> builder)
        {
            builder.ToTable("CharityAidTypes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameEn).HasMaxLength(100);

            builder.HasData(
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeCash, NameAr = "نقدي", NameEn = "Cash", DisplayOrder = 1, IsActive = true },
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeFood, NameAr = "غذائي", NameEn = "Food", DisplayOrder = 2, IsActive = true },
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeMedical, NameAr = "علاجي", NameEn = "Medical", DisplayOrder = 3, IsActive = true },
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeEducational, NameAr = "تعليمي", NameEn = "Educational", DisplayOrder = 4, IsActive = true },
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeClothes, NameAr = "ملابس", NameEn = "Clothes", DisplayOrder = 5, IsActive = true },
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeDevices, NameAr = "أجهزة", NameEn = "Devices", DisplayOrder = 6, IsActive = true },
                new AidTypeLookup { Id = CharityLookupSeedIds.AidTypeSponsorship, NameAr = "كفالة", NameEn = "Sponsorship", DisplayOrder = 7, IsActive = true });
        }
    }
}
