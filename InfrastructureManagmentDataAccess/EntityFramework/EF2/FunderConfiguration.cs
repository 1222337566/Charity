using InfrastrfuctureManagmentCore.Domains.Charity.Funders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class FunderConfiguration : IEntityTypeConfiguration<Funder>
    {
        public void Configure(EntityTypeBuilder<Funder> builder)
        {
            builder.ToTable("CharityFunders");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).HasMaxLength(50).IsRequired();
            builder.Property(x => x.Name).HasMaxLength(250).IsRequired();
            builder.Property(x => x.FunderType).HasMaxLength(100).IsRequired();
            builder.Property(x => x.ContactPerson).HasMaxLength(200);
            builder.Property(x => x.PhoneNumber).HasMaxLength(50);
            builder.Property(x => x.Email).HasMaxLength(200);
            builder.Property(x => x.AddressLine).HasMaxLength(1000);
            builder.Property(x => x.Notes).HasMaxLength(4000);

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.Name);
            builder.HasIndex(x => new { x.FunderType, x.IsActive });
        }
    }
}
