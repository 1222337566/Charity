using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class BeneficiaryStatusLookupConfiguration : IEntityTypeConfiguration<BeneficiaryStatusLookup>
    {
        public void Configure(EntityTypeBuilder<BeneficiaryStatusLookup> builder)
        {
            builder.ToTable("CharityBeneficiaryStatuses");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(100);
            builder.Property(x => x.NameEn).HasMaxLength(100);

            builder.HasData(
                new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusNew, NameAr = "جديد", NameEn = "New", DisplayOrder = 1, IsActive = true },
                new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusUnderReview, NameAr = "تحت الدراسة", NameEn = "Under Review", DisplayOrder = 2, IsActive = true },
                new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusApproved, NameAr = "معتمد", NameEn = "Approved", DisplayOrder = 3, IsActive = true },
                new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusSuspended, NameAr = "موقوف", NameEn = "Suspended", DisplayOrder = 4, IsActive = true },
                new BeneficiaryStatusLookup { Id = CharityLookupSeedIds.BeneficiaryStatusRejected, NameAr = "مرفوض", NameEn = "Rejected", DisplayOrder = 5, IsActive = true });
        }
    }
}
