using InfrastrfuctureManagmentCore.Domains.Charity.Kafala;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF.Charity.Kafala
{
    public class KafalaSponsorConfiguration : IEntityTypeConfiguration<KafalaSponsor>
    {
        public void Configure(EntityTypeBuilder<KafalaSponsor> builder)
        {
            builder.ToTable("CharityKafalaSponsors");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.SponsorCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.FullName).HasMaxLength(250).IsRequired();
            builder.Property(x => x.SponsorType).HasMaxLength(50).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(30);
            builder.Property(x => x.Email).HasMaxLength(150);
            builder.Property(x => x.NationalIdOrTaxNo).HasMaxLength(50);
            builder.HasIndex(x => x.SponsorCode).IsUnique();
        }
    }
}
