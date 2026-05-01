using InfrastrfuctureManagmentCore.Domains.Charity.Lookups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class GovernorateConfiguration : IEntityTypeConfiguration<Governorate>
    {
        public void Configure(EntityTypeBuilder<Governorate> builder)
        {
            builder.ToTable("CharityGovernorates");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.NameAr).IsRequired().HasMaxLength(150);
            builder.Property(x => x.NameEn).HasMaxLength(150);

            builder.HasIndex(x => x.NameAr).IsUnique();
        }
    }
}
