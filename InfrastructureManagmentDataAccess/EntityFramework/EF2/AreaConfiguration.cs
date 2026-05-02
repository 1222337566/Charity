using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class AreaConfiguration : IEntityTypeConfiguration<Area>
    {
        public void Configure(EntityTypeBuilder<Area> builder)
        {
            builder.ToTable("CharityAreas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
            builder.Property(x => x.NameEn).HasMaxLength(150);

            builder.HasOne(x => x.City)
                .WithMany(x => x.Areas)
                .HasForeignKey(x => x.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.CityId, x.NameAr }).IsUnique();
        }
    }
}
