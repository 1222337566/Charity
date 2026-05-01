using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.ToTable("CharityCities");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
            builder.Property(x => x.NameEn).HasMaxLength(150);

            builder.HasOne(x => x.Governorate)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.GovernorateId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.GovernorateId, x.NameAr }).IsUnique();
        }
    }
}
