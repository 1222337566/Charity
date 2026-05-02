using InfrastrfuctureManagmentCore.Domains.Charity.Funding;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class GrantorConfiguration : IEntityTypeConfiguration<Grantor>
    {
        public void Configure(EntityTypeBuilder<Grantor> builder)
        {
            builder.ToTable("Grantors");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.GrantorCode).HasMaxLength(50).IsRequired();
            builder.Property(x => x.NameAr).HasMaxLength(250).IsRequired();
            builder.Property(x => x.NameEn).HasMaxLength(250);
            builder.Property(x => x.ContactPerson).HasMaxLength(200);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.Property(x => x.Address).HasMaxLength(500);
            builder.Property(x => x.Notes).HasMaxLength(1000);
            builder.Property(x => x.CreatedByUserId).HasMaxLength(450);

            builder.HasIndex(x => x.GrantorCode).IsUnique();
            builder.HasIndex(x => x.NameAr);
            builder.HasIndex(x => x.IsActive);
        }
    }
}
