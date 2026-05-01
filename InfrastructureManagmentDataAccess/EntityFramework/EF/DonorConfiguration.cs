using InfrastructureManagmentCore.Domains.Identity;
using InfrastrfuctureManagmentCore.Domains.Charity.Donors;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InfrastructureManagmentDataAccess.EntityFramework.EF
{
    public class DonorConfiguration : IEntityTypeConfiguration<Donor>
    {
        public void Configure(EntityTypeBuilder<Donor> builder)
        {
            builder.ToTable("CharityDonors");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Code).IsRequired().HasMaxLength(30);
            builder.Property(x => x.DonorType).IsRequired().HasMaxLength(50);
            builder.Property(x => x.FullName).IsRequired().HasMaxLength(250);
            builder.Property(x => x.ContactPerson).HasMaxLength(250);
            builder.Property(x => x.NationalIdOrTaxNo).HasMaxLength(30);
            builder.Property(x => x.PhoneNumber).HasMaxLength(30);
            builder.Property(x => x.Email).HasMaxLength(250);
            builder.Property(x => x.AddressLine).HasMaxLength(500);
            builder.Property(x => x.PreferredCommunicationMethod).HasMaxLength(50);
            builder.Property(x => x.Notes).HasMaxLength(2000);

            builder.HasIndex(x => x.Code).IsUnique();
            builder.HasIndex(x => x.PhoneNumber);
            builder.HasIndex(x => x.NationalIdOrTaxNo).IsUnique().HasFilter("[NationalIdOrTaxNo] IS NOT NULL");

            builder.HasOne(x => x.Governorate).WithMany().HasForeignKey(x => x.GovernorateId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.City).WithMany().HasForeignKey(x => x.CityId).OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(x => x.Area).WithMany().HasForeignKey(x => x.AreaId).OnDelete(DeleteBehavior.Restrict);
        }
    }
}
